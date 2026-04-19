using System;
using System.IO;
using System.Reflection;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using ExamPaper.Infrastructure.Exporter;
using ExamPaper.Infrastructure.Repositories;
using ExamPaper.Service.Factories;
using ExamPaper.Service.Generator;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string version = "v1";
const string frontendExamPaperPolicy = "FrontendExamPaperPolicy";

var builder = WebApplication.CreateBuilder();

string projectRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
string dataFolder = Path.Combine(projectRoot, "Data");
string filePath = Path.Combine(dataFolder, "Blank.json");

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>((_) => new QuestionRepository(filePath));
builder.Services.AddScoped<IQuestionRepository>(sp => new QuestionRepository(filePath));
builder.Services.AddScoped<IQuestionProvider>(sp => sp.GetRequiredService<IQuestionRepository>());

builder.Services.AddScoped<IExamPaperFactory, ExamPaperFactory>();
builder.Services.AddScoped<IExamGenerator, ExamPaperGenerator>();
builder.Services.AddScoped<JsonExamExporter>();
builder.Services.AddScoped<PdfExamExporter>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendExamPaperPolicy, policy =>
    {
        policy.WithOrigins()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/openapi/{version}.json", version); });
}

app.UseHttpsRedirection();
app.UseCors(frontendExamPaperPolicy);
app.MapGet("/api/questions", (IQuestionRepository repo) =>
    repo.GetAllQuestions());
app.MapGet("/api/questions/{id:guid}", (Guid id, IQuestionRepository repo) =>
{
    var question = repo.GetQuestionById(id);
    return question != null ? Results.Ok(question) : Results.NotFound();
});

app.MapPost("/api/exam/generate", (GenerationSettings settings, IExamGenerator generator) =>
{
    try
    {
        var tickets = generator.Generate(settings);
        return Results.Ok(tickets);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/exam/export/pdf", (
    [FromKeyedServices("pdf")] PdfExamExporter exporter, IExamGenerator generator, GenerationSettings settings) =>
{
    try
    {
        var tickets = generator.Generate(settings);
        byte[] file = exporter.Export(tickets);
        return Results.File(file, "exporter/pdf", "Blank.pdf");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/exam/export/json", (
    [FromKeyedServices("json")] JsonExamExporter exporter, IExamGenerator generator, GenerationSettings settings) =>
{
    try
    {
        var tickets = generator.Generate(settings);
        byte[] file = exporter.Export(tickets);
        return Results.File(file, "exporter/json", "Blank.json");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
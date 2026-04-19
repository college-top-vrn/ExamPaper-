using System;
using System.IO;
using System.Reflection;

using ExamPaper.Core.Interfaces;
using ExamPaper.Infrastructure.Repositories;

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

app.Run();
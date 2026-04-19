using System;
using System.IO;
using System.Linq;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using ExamPaper.Infrastructure.Exporter;
using ExamPaper.Infrastructure.Repositories;
using ExamPaper.Service.DTOs;
using ExamPaper.Service.Factories;
using ExamPaper.Service.Generator;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string version = "v1";
const string name = "ExamPaperGenerator";
var builder = WebApplication.CreateBuilder();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string solutionDirectory = Directory.GetParent(builder.Environment.ContentRootPath)?.FullName
                           ?? builder.Environment.ContentRootPath;
string filePath = Path.Combine(solutionDirectory, "Data", "Blank.json");


builder.Services.AddScoped<IQuestionRepository, QuestionRepository>(_ => new QuestionRepository(filePath));
builder.Services.AddScoped<IQuestionProvider>(sp => sp.GetRequiredService<IQuestionRepository>());

builder.Services.AddScoped<IQuestionFactory, QuestionFactory>();
builder.Services.AddScoped<IExamPaperFactory, ExamPaperFactory>();
builder.Services.AddScoped<IExamGenerator, ExamPaperGenerator>();

builder.Services.AddScoped<PdfExamExporter>();
builder.Services.AddScoped<JsonExamExporter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{name}");
    });
}

app.UseHttpsRedirection();


app.MapGet("/api/questions", (IQuestionRepository repo) =>
    {
        var questions = repo.GetAllQuestions();
        return Results.Ok(questions);
    })
    .WithName("GetAllQuestions")
    .WithTags("Questions");

app.MapGet("/api/questions/{id:guid}", (Guid id, IQuestionRepository repo) =>
    {
        var question = repo.GetQuestionById(id);
        return question is not null ? Results.Ok(question) : Results.NotFound(new { error = "Вопрос не найден." });
    })
    .WithName("GetQuestionById")
    .WithTags("Questions");

app.MapPost("/api/questions", (
        [FromBody] CreateQuestionDto dto,
        IQuestionRepository repo,
        IQuestionFactory factory) =>
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
        {
            return Results.BadRequest(new { error = "Текст вопроса не может быть пустым." });
        }

        try
        {
            var newId = Guid.NewGuid();
            var newQuestion = factory.CreateQuestion(newId, dto.Text);

            repo.AddQuestion(newQuestion);
            repo.SaveChanges();

            return Results.Created($"/api/questions/{newId}", newQuestion);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
    })
    .WithName("CreateQuestion")
    .WithTags("Questions");

app.MapDelete("/api/questions/{id:guid}", (Guid id, IQuestionRepository repo) =>
    {
        var question = repo.GetQuestionById(id);
        if (question is null)
        {
            return Results.NotFound(new { error = "Вопрос не найден." });
        }

        repo.RemoveQuestion(id);
        repo.SaveChanges();

        return Results.NoContent();
    })
    .WithName("DeleteQuestion")
    .WithTags("Questions");


app.MapPost("/api/exam/generate", (
        [FromBody] GenerationSettings settings,
        IExamGenerator generator) =>
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
    })
    .WithName("GenerateExamPapers")
    .WithTags("Exams");

app.MapPost("/api/exam/export/pdf", (
        [FromBody] GenerationSettings settings,
        IExamGenerator generator,
        PdfExamExporter exporter) =>
    {
        try
        {
            var tickets = generator.Generate(settings).ToList();
            byte[] fileBytes = exporter.Export(tickets);
            return Results.File(fileBytes, "application/pdf", "exams.pdf");
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    })
    .WithName("ExportExamPdf")
    .WithTags("Exams");

app.MapPost("/api/exam/export/json", (
        [FromBody] GenerationSettings settings,
        IExamGenerator generator,
        JsonExamExporter exporter) =>
    {
        try
        {
            var tickets = generator.Generate(settings).ToList();
            byte[] fileBytes = exporter.Export(tickets);
            return Results.File(fileBytes, "application/json", "exams.json");
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    })
    .WithName("ExportExamJson")
    .WithTags("Exams");

app.Run();
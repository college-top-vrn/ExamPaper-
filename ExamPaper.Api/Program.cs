using System;

using ExamPaper.Core.Interfaces;
using ExamPaper.Infrastructure.Repositories;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string version = "v1";
const string localHost = "http://localhost:5047";
const string frontendExamPaperPolicy = "FrontendExamPaperPolicy";

var builder = WebApplication.CreateBuilder();

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendExamPaperPolicy, policy =>
    {
        policy.WithOrigins(localHost)
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
/// <summary>
/// GET /api/questions
/// Получает все вопросы экзамена.
/// </summary>
/// <param name="repo">Репозиторий вопросов, автоматически инжектируется DI контейнером.</param>
/// <returns>Список всех вопросов в формате JSON.</returns>
/// <response code="200">Успешно возвращены все вопросы.</response>
app.MapGet("/api/questions", (IQuestionRepository repo) => 
    repo.GetAllQuestions());
/// <summary>
/// GET /api/questions/{id:guid}
/// Получает вопрос по уникальному идентификатору.
/// </summary>
/// <param name="id">Уникальный идентификатор вопроса (GUID).</param>
/// <param name="repo">Репозиторий вопросов, автоматически инжектируется DI контейнером.</param>
/// <returns>Вопрос в формате JSON или 404 если не найден.</returns>
/// <response code="200">Вопрос успешно найден и возвращен.</response>
/// <response code="404">Вопрос с указанным ID не найден.</response>
app.MapGet("/api/questions/{id:guid}", (Guid id, IQuestionRepository repo) =>
{
    var question = repo.GetQuestionById(id);
    return question != null ? Results.Ok(question) : Results.NotFound();
});

app.Run();
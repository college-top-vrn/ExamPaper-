using System;
using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string version = "v1";
const string localHost = "http://localhost:5047";
const string frontendExamPaperPolicy = "FrontendExamPaperPolicy";

var builder = WebApplication.CreateBuilder();


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

using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.Extensions.Options;
using backend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   
            .AllowAnyMethod()  
            .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors();

const string connectionString = "Data Source=quiz.db";

DbInitializer.Init(connectionString);

app.MapQuizEndpoints(connectionString);

app.Run();

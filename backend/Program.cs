using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.Extensions.Options;
using static backend.DbInitialazer;
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

const string connectionString = "Data Siurce=quiz.db";

DbInitialazer.Init(connectionString);

app.MapQuizEndpoints(connectionString);

app.Run();

using backend.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace backend;

public static class QuizEndpoints
{
    public static void MapQuizEndpoints(this WebApplication app, string connectionString)
    {
        GetCategories(app, connectionString);
        GetQuize(app, connectionString);
        QuizeCheck(app, connectionString);
    }

    private static void GetCategories(WebApplication app, string connectionString)
    {
        app.MapGet("/api/categories", async () =>
        {
            using var connection = new SqliteConnection(connectionString);
            var sql = "SELECT * FROM Categories";
            var categories = await connection.QueryAsync<Category>(sql);
            return Results.Ok(categories);
        });
    }

    private static void GetQuize(WebApplication app, string connectionString)
    {
        app.MapGet("/api/quize/level", async (int[] ids) =>
        {
            using var connection = new SqliteConnection(connectionString);
            var correctSql = "SELECT * FROM Persons WHERE CateegoryId in @Ids ORDER BY RANDOM() LIMIT 1";
            var correct = await connection.QueryFirstOrDefaultAsync<Person>(correctSql, new { Ids = ids });

            if (correct == null)
                return Results.BadRequest("Кактегория пуста");

            var distractorsSql = @"
            SELECT ImageUrl
            FROM Persons 
            WHERE Period >= @MinPeriod AND Period <= @MaxPeriod
            ORDER BY RANDOM()
            LIMIT 2;
            ";

            var distractors = await connection.QueryAsync<string>(distractorsSql, new
            {
                MinPeriod = correct.Period - 3,
                MaxPeriod = correct.Period + 2
            });

            var images = distractors.ToList();
            images.Add(correct.ImageUrl);
            var shuffledImages = images.OrderBy(_ => Guid.NewGuid()).ToList();

            return Results.Ok(new
            {
                description = correct.Description,
                correctName = correct.Name,
                fact = correct.Fact,
                imageOptions = shuffledImages
            });
        });
    }

    private static void QuizeCheck(WebApplication app, string connectionString)
    {
        app.MapGet("api/quize/ckeck", async (QuizCheckRequest request) =>
        {
            using var connection = new SqliteConnection(connectionString);

            var sql = "SELECT ImageUrl FROM Persons WHERE id = @Id";
            var correctUrl = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Id = request.CorrectObjectId });

            if (correctUrl == null)
                return Results.BadRequest($"картинка по id:{request.CorrectObjectId} не найдена")

            return Results.Ok(new { success = correctUrl == request.ImageUrl });
        });
    }
}
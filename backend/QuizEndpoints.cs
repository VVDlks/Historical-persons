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
        GetQuizLevels(app, connectionString);
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
            var correctSql = "SELECT * FROM Persons WHERE CategoryId in @Ids ORDER BY RANDOM() LIMIT 1";
            var correct = await connection.QueryFirstOrDefaultAsync<Person>(correctSql, new { Ids = ids });

            if (correct == null)
                return Results.BadRequest("Кактегория пуста");

            var distractorsSql = @"
            SELECT ImageUrl
            FROM Persons 
            WHERE (Period >= @MinPeriod AND Period <= @MaxPeriod)
                AND id != @CorrectId
            ORDER BY RANDOM()
            LIMIT 3;
            ";

            var distractors = await connection.QueryAsync<string>(distractorsSql, new
            {
                MinPeriod = correct.Period - 3,
                MaxPeriod = correct.Period + 2,
                CorrectId = correct.Id
            });

            var images = distractors.ToList();
            images.Add(correct.ImageUrl);
            var shuffledImages = images.OrderBy(_ => Guid.NewGuid()).ToList();

            return Results.Ok(new
            {
                description = correct.Description,
                correctName = correct.Name,
                fact = correct.Fact,
                imageOptions = shuffledImages,
                correctImageUrl = correct.ImageUrl
            });
        });
    }

    private static void QuizeCheck(WebApplication app, string connectionString)
    {
        app.MapPost("api/quize/ckeck", async (QuizCheckRequest request) =>
        {
            using var connection = new SqliteConnection(connectionString);

            var sql = "SELECT ImageUrl FROM Persons WHERE id = @Id";
            var correctUrl = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Id = request.CorrectObjectId });

            if (correctUrl == null)
                return Results.BadRequest($"картинка по id:{request.CorrectObjectId} не найдена");

            return Results.Ok(new { success = correctUrl == request.ImageUrl });
        });
    }

    private static void GetQuizLevels(WebApplication app, string connectionString)
    {
        app.MapGet("api/levels", async () =>
        {
            using var connection = new SqliteConnection(connectionString);

            var sql = @"
            SELECT ql.Id, ql.Title, qlc.CategoryId
            FROM QuizLevels ql
            JOIN QuizLevelCategories qlc ON ql.id = qlc.QuizLevelId;
            ";

            var rawRows = await connection.QueryAsync<FlatLevelRow>(sql);

            var levels = rawRows
            .GroupBy(r => new { r.Id, r.Title })
            .Select(g => new
            {
                id = g.Key.Id,
                title = g.Key.Title,
                categoryIds = g.Select(r => r.CategoryId).ToArray()
            });

            return Results.Ok(levels);
        });
    }

    private record FlatLevelRow(long Id, string Title, long CategoryId);
}


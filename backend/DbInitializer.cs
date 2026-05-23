using Microsoft.Data.Sqlite;
using Dapper;

namespace backend;

public static class DbInitializer
{
    public static void Init(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ObjectCategory VARCHAR(255) NOT NULL
            );
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Persons (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name VARCHAR(255) NOT NULL,
                Description TEXT NOT NULL,
                Fact TEXT NOT NULL,
                ImageUrl TEXT NOT NULL,
                CategoryId INTEGER,
                Period INTEGER NOT NULL,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
            );
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS QuizLevels (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title VARCHAR(255) NOT NULL
            );
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS QuizLevelCategories (
                QuizLevelId INTEGER,
                CategoryId INTEGER,
                PRIMARY KEY (QuizLevelId, CategoryId),
                FOREIGN KEY (QuizLevelId) REFERENCES QuizLevels(Id) ON DELETE CASCADE,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
            );
        ");
    }
}
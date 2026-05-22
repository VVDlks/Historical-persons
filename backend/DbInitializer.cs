using Microsoft.Data.Sqlite;
using Dapper;

namespace backend;

public static class DbInitialazer
{
    public static void Init(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Categories (
                Id INTEGER PRIMARY KEY AUTHOINCREMENT,
                ObjectCategory VARCHAR(255) NOT NULL,
            );
        ");

        connection.Execute(@"
            CREATE TABLE Persons (
                Id INTEGER PRIMARY KEY AUTHOINCREMENT,
                Name VARCHAR(255) NOT NULL,
                Description TEXT NOT NULL,
                Fact TEXT NOT NULL,
                ImageUrl TEXT NOT NULL,
                CategoryId INTEGER FOREIGN KEY
                Period INTEGER NOT NULL
            )
        ");
    }
}
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

        var count = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM Categories");
        if (count == 0)
        {
            connection.Execute(@"
                INSERT INTO Categories (Id, ObjectCategory) VALUES 
                (1, 'Правители и политики'), 
                (2, 'Ученые и изобретатели'), 
                (3, 'Творцы и мыслители')");

            connection.Execute(@"
                INSERT INTO QuizLevels (Id, Title) VALUES 
                (1, 'Правители и политики'), 
                (2, 'Ученые и изобретатели'), 
                (3, 'Философы и мыслители'), 
                (4, 'Исторические личности')");

            connection.Execute(@"
                INSERT INTO QuizLevelCategories (QuizLevelId, CategoryId) VALUES 
                (1, 1), 
                (2, 2), 
                (3, 3),
                (4, 1), (4, 2), (4, 3);
            ");
        }

        var personsCount = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM Persons");
        if (personsCount == 0)
        {
            string filePath = "persons.tsv"; 
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) 
                        continue;

                    var parts = line.Split('\t');

                    if (parts.Length >= 6)
                    {
                        var name = parts[0].Trim();
                        var description = parts[1].Trim();
                        var fact = parts[2].Trim();
                        var imageUrl = parts[3].Trim();

                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description)) 
                            continue;

                        var period = int.Parse(parts[4].Trim());
                        var categoryId = int.Parse(parts[5].Trim());

                        connection.Execute(@"
                            INSERT INTO Persons (Name, Description, Fact, ImageUrl, Period, CategoryId) 
                            VALUES (@Name, @Description, @Fact, @ImageUrl, @Period, @CategoryId)",
                            new { Name = name, Description = description, Fact = fact, ImageUrl = imageUrl, Period = period, CategoryId = categoryId });
                    }
                }
            }
        }
    }
}
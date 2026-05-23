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
            connection.Execute("INSERT INTO Categories (ObjectCategory) VALUES ('Писатели'), ('Ученые')");
            connection.Execute(@"
                INSERT INTO Persons (Name, Description, Fact, ImageUrl, CategoryId, Period) VALUES 
                ('Лев Толстой', 'Русский писатель и мыслитель.', 'Написал «Войну и мир».', 'tolstoy.jpg', 1, 19),
                ('Фёдор Достоевский', 'Русский писатель, мыслитель, философ и публицист.', 'Автор романа «Преступление и наказание».', 'dostoevsky.jpg', 1, 19),
                ('Антон Чехов', 'Русский писатель, прозаик, драматург.', 'По профессии врач.', 'chekhov.jpg', 1, 19),
                ('Александр Пушкин', 'Русский поэт, драматург и прозаик.', 'Наше всё.', 'pushkin.jpg', 1, 19),
                ('Альберт Эйнштейн', 'Физик-теоретик.', 'Автор теории относительности.', 'einstein.jpg', 2, 20),
                ('Иван Павлов', 'Учёный, физиолог.', 'Создатель науки о высшей нервной деятельности.', 'pavlov.jpg', 2, 20),
                ('Никола Тесла', 'Изобретатель в области электротехники.', 'Известен вкладом в создание устройств на переменном токе.', 'tesla.jpg', 2, 19),
                ('Мария Склодовская-Кюри', 'Учёный-экспериментатор.', 'Первая женщина — лауреат Нобелевской премии.', 'curie.jpg', 2, 19);
            ");
            connection.Execute("INSERT INTO QuizLevels (Title) VALUES ('Литература'), ('Наука')");
            connection.Execute("INSERT INTO QuizLevelCategories (QuizLevelId, CategoryId) VALUES (1, 1), (2, 2)");
        }
    }
}
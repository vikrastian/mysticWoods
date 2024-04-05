using System;
using System.Collections.Generic; // Ensure this using directive is added for List<>
using Microsoft.Data.Sqlite;

public static class DatabaseManager
{
    private static string dbFileName = "gameDatabase.db";

    public static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();

            var tableCreationCommand = connection.CreateCommand();
            tableCreationCommand.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Progress (
                    ProgressID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER NOT NULL,
                    LifeLevel INTEGER NOT NULL,
                    Distance INTEGER NOT NULL,
                    FOREIGN KEY (UserID) REFERENCES Users(UserID)
                );

                CREATE TABLE IF NOT EXISTS HighScores (
                    UserID INTEGER PRIMARY KEY,
                    Score INTEGER NOT NULL,
                    FOREIGN KEY (UserID) REFERENCES Users(UserID)
                );
            ";
            tableCreationCommand.ExecuteNonQuery();
        }
    }

    public static void SaveProgress(string name, int lifeLevel, int distance)
    {
        int userId = EnsureUserExists(name);

        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Progress (UserID, LifeLevel, Distance) VALUES (@UserID, @LifeLevel, @Distance)";
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@LifeLevel", lifeLevel);
            command.Parameters.AddWithValue("@Distance", distance);
            command.ExecuteNonQuery();
        }
    }

    public static (int? LifeLevel, int? Distance) LoadProgress(string name)
    {
        int userId = EnsureUserExists(name, checkOnly: true);

        if (userId == -1)
        {
            return (null, null);
        }

        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT LifeLevel, Distance FROM Progress WHERE UserID = @UserID ORDER BY ProgressID DESC LIMIT 1";
            command.Parameters.AddWithValue("@UserID", userId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return (reader.GetInt32(0), reader.GetInt32(1));
                }
            }
        }
        return (null, null);
    }

    private static int EnsureUserExists(string name, bool checkOnly = false)
    {
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();

            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = "SELECT UserID FROM Users WHERE Name = @Name";
            checkCommand.Parameters.AddWithValue("@Name", name);

            using (var reader = checkCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }

            if (checkOnly)
            {
                return -1;
            }

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Users (Name, Password) VALUES (@Name, '123')";
            insertCommand.Parameters.AddWithValue("@Name", name);
            insertCommand.ExecuteNonQuery();

            using (var cmd = new SqliteCommand("SELECT last_insert_rowid()", connection))
            {
                return (int)(long)cmd.ExecuteScalar();
            }
        }
    }

    public static void SaveHighScore(string name, int score)
    {
        int userId = EnsureUserExists(name);
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            try
            {
                connection.Open();
                var upsertCommand = connection.CreateCommand();
                upsertCommand.CommandText =
                    @"
                INSERT INTO HighScores (UserID, Score) VALUES (@UserID, @Score)
                ON CONFLICT(UserID) DO UPDATE SET Score = CASE WHEN Score < @Score THEN @Score ELSE Score END
                ";
                upsertCommand.Parameters.AddWithValue("@UserID", userId);
                upsertCommand.Parameters.AddWithValue("@Score", score);

                // Debug output
                Console.WriteLine($"Trying to save score {score} for user {name} with userID {userId}");

                upsertCommand.ExecuteNonQuery();

                // Debug output
                Console.WriteLine("Score saved successfully.");
            }
            catch (Exception ex)
            {
                // Exception handling
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }

    // public static void SaveHighScore(string name, int score)
    // {
    //     int userId = EnsureUserExists(name);
    //     using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
    //     {
    //         connection.Open();
    //         var upsertCommand = connection.CreateCommand();
    //         upsertCommand.CommandText =
    //             @"
    //             INSERT INTO HighScores (UserID, Score) VALUES (@UserID, @Score)
    //             ON CONFLICT(UserID) DO UPDATE SET Score = CASE WHEN Score < @Score THEN @Score ELSE Score END
    //             ";
    //         upsertCommand.Parameters.AddWithValue("@UserID", userId);
    //         upsertCommand.Parameters.AddWithValue("@Score", score);
    //         upsertCommand.ExecuteNonQuery();
    //     }
    // }

    public static int GetHighScore(string name)
    {
        int userId = EnsureUserExists(name, checkOnly: true);
        if (userId == -1) return 0;

        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Score FROM HighScores WHERE UserID = @UserID";
            command.Parameters.AddWithValue("@UserID", userId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
        }
        return 0;
    }

    public static List<string> GetTopThreeHighScores()
    {
        var topScores = new List<string>();
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT u.Name, hs.Score
                FROM HighScores hs
                JOIN Users u ON hs.UserID = u.UserID
                ORDER BY hs.Score DESC
                LIMIT 3
                ";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    int score = reader.GetInt32(1);
                    topScores.Add($"{name}: {score}");
                }
            }
        }
        return topScores;
    }
}
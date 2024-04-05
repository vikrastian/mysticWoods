using System;
using Microsoft.Data.Sqlite; // Lagt til i cmd: dotnet add package Microsoft.Data.Sqlite

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
                    Name TEXT NOT NULL,
                    Password TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Progress (
                    ProgressID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER NOT NULL,
                    LifeLevel INTEGER NOT NULL,
                    Distance INTEGER NOT NULL,
                    FOREIGN KEY (UserID) REFERENCES Users(UserID)
                );
            ";
            tableCreationCommand.ExecuteNonQuery();
        }
    }

    // Save game progress for a user
    public static void SaveProgress(string name, int lifeLevel, int distance)
    {
        int userId = EnsureUserExists(name);

        if (userId == -1) // Check if user was not found and not created
        {
            Console.WriteLine("User does not exist, cannot save progress.");
            return; // Exit the method if user doesn't exist
        }

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

    // Load the latest game progress for a user
    public static (int? LifeLevel, int? Distance) LoadProgress(string name)
    {
        int userId = EnsureUserExists(name, checkOnly: true); // Modified to use checkOnly parameter

        if (userId == -1) // If user does not exist, return nulls.
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
                if (reader != null && reader.Read())
                {
                    int lifeLevel = reader.IsDBNull(0) ? 100 : reader.GetInt32(0);
                    int distance = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                    return (lifeLevel, distance);
                }
            }
        }

        // Return null if no progress found
        return (null, null);
    }

    // Ensure a user exists in the database and return the UserID, with an option to only check for existence
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
                if (reader != null && reader.Read())
                {
                    return reader.GetInt32(0); // Return UserID if user exists
                }
            }

            if (checkOnly)
            {
                return -1; // Return -1 if user not found and checkOnly is true
            }

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Users (Name, Password) VALUES (@Name, @Password)";
            insertCommand.Parameters.AddWithValue("@Name", name);
            insertCommand.Parameters.AddWithValue("@Password", "123");
            insertCommand.ExecuteNonQuery();

            using (var cmd = new SqliteCommand("SELECT last_insert_rowid()", connection))
            {
                var result = cmd.ExecuteScalar();
                return (int)(result is long id ? id : -1); // Return new UserID or -1 if an error occurred
            }
        }
    }
}
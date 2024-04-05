using System;
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
        // First, find or create the user in the database
        int userId = EnsureUserExists(name);

        // Then, insert the progress record
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
    public static (int LifeLevel, int Distance) LoadProgress(string name)
    {
        int userId = EnsureUserExists(name);

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
                    int lifeLevel = reader.IsDBNull(0) ? 100 : reader.GetInt32(0); // Use default value if null
                    int distance = reader.IsDBNull(1) ? 0 : reader.GetInt32(1); // Use default value if null
                    return (lifeLevel, distance);
                }
            }
        }

        // Return default values if no progress found
        return (100, 0);
    }

    // Ensure a user exists in the database and return the UserID
    private static int EnsureUserExists(string name)
    {
        using (var connection = new SqliteConnection($"Data Source={dbFileName}"))
        {
            connection.Open();

            // Check if user already exists
            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = "SELECT UserID FROM Users WHERE Name = @Name";
            checkCommand.Parameters.AddWithValue("@Name", name);

            using (var reader = checkCommand.ExecuteReader())
            {
                if (reader != null && reader.Read())
                {
                    return reader.GetInt32(0); // Non-nullable, assuming UserID is never null
                }
            }

            // If not, insert new user
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Users (Name, Password) VALUES (@Name, @Password)";
            insertCommand.Parameters.AddWithValue("@Name", name);
            insertCommand.Parameters.AddWithValue("@Password", "123");
            insertCommand.ExecuteNonQuery();

            // Retrieve and return the new UserID
            long userId;
            using (var cmd = new SqliteCommand("SELECT last_insert_rowid()", connection))
            {
                var result = cmd.ExecuteScalar();
                userId = result is long id ? id : -1; // Checks if 'result' is of type long and assigns its value to 'userId' if it is; otherwise assigns -1 to indicate an error or absence of data.
 
            }
            return (int)userId;
        }
    }

}

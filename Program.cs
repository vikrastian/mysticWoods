using System;

class Program
{
    static void Main(string[] args)
    {
        DatabaseManager.InitializeDatabase();
        Console.Clear();

        // Display welcome message and top 3 high scores at the beginning of the game
        Console.WriteLine("Welcome to the game Mystic Woods. \nDo you have what it takes to beat our high scores?");
        Console.WriteLine();

        // Display top 3 high scores at the beginning of the game
        DisplayTopScores();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        Console.Clear();
        Console.WriteLine("Do you want to restore a saved game? (yes/no)");
        string restoreChoice = Console.ReadLine()?.ToLower() ?? "";
        MainCharacter? player = null;
        int distance = 0;

        if (restoreChoice == "yes")
        {
            Console.Clear();
            Console.WriteLine("Please enter your username:");
            Console.WriteLine();

            string? username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.Clear();
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            var (lifeLevel, distanceLoaded) = DatabaseManager.LoadProgress(username);
            if (lifeLevel.HasValue && distanceLoaded.HasValue)
            {
                player = new MainCharacter(username, lifeLevel.Value);
                distance = distanceLoaded.Value;
                Console.Clear();
                Console.WriteLine($"Welcome back {player.Name}. Your lifeLevel is {player.LifeLevel}. You have covered a distance of {distance} meters in the forest. Move carefully and stay safe!");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("User not found. Game has been terminated.");
                return;
            }
        }

        if (player == null)
        {
            Console.Clear();
            Console.WriteLine("What is your name?");
            string? characterName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(characterName))
            {
                Console.Clear();
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            player = new MainCharacter(characterName, 100);
            DatabaseManager.SaveProgress(player.Name, player.LifeLevel, 0);
            Console.Clear();
            Console.WriteLine($"Welcome {player.Name}! \nYour life level is {player.LifeLevel}.");
            Console.WriteLine("\nObjective: Survive walking 500m in the forest!");
            Console.WriteLine("\nINFO: \nAuto save has been performed. \nClick 's' to save during the game and 'q' to quit. \nRemember your user name to restore and continue later.");
        }

        Console.WriteLine("\nWalk using keyboard arrows.");

        bool isRunning = true;
        while (isRunning)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            string direction = "";

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    direction = "forward";
                    break;
                case ConsoleKey.LeftArrow:
                    direction = "left";
                    break;
                case ConsoleKey.RightArrow:
                    direction = "right";
                    break;
            }

            if (!string.IsNullOrEmpty(direction))
            {
                isRunning = Navigation.NavigateAndCheckForEncounter(direction, player, ref distance);
                Console.WriteLine($"Total distance moved: {distance} meters.");
            }

            if (keyInfo.Key == ConsoleKey.S)
            {
                Console.Clear();
                DatabaseManager.SaveProgress(player.Name, player.LifeLevel, distance);
                Console.WriteLine("Game saved successfully!");
            }
            else if (keyInfo.Key == ConsoleKey.Q)
            {
                Console.Clear();
                DatabaseManager.SaveHighScore(player.Name, distance); // Saving the high score upon quitting
                Console.WriteLine("Exiting the game. Stay safe in the real world!");
                isRunning = false;
            }
        }

        // Optionally display high score here
        var highScore = DatabaseManager.GetHighScore(player.Name);
        Console.WriteLine($"Your high score is: {highScore} meters. Well done!");
    }

    static void DisplayTopScores()
    {
        var topThreeScores = DatabaseManager.GetTopThreeHighScores();
        Console.WriteLine("Top 3 High Scores:");
        foreach (var score in topThreeScores)
        {
            Console.WriteLine(score);
        }
        Console.WriteLine(); // Add a newline for better formatting
    }
}
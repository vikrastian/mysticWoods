using System;

class Program
{
    static void Main(string[] args)
    {
        DatabaseManager.InitializeDatabase();

        Console.WriteLine("Do you want to restore a saved game? (yes/no)");
        string restoreChoice = Console.ReadLine()?.ToLower() ?? "";
        MainCharacter? player = null;
        int distance = 0;

        if (restoreChoice == "yes")
        {
            Console.WriteLine();
            Console.WriteLine("Please enter your username:");
            string? username = Console.ReadLine(); // Added ? to say: Im aware it can be null, I deal with it later (in my if statement).

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            var (lifeLevel, distanceLoaded) = DatabaseManager.LoadProgress(username);
            if (lifeLevel > 0)
            {
                player = new MainCharacter(username, lifeLevel);
                distance = distanceLoaded;
                Console.WriteLine();
                Console.WriteLine($"Welcome back {player.Name}. Your lifeLevel is {player.LifeLevel} and you have covered a distance of {distance} meters in the forest. Move carefully and stay safe!");
            }
            else
            {
                Console.WriteLine("No saved game found for this username, starting a new game.");
            }
        }

        if (player == null) // This means either they chose not to restore, or the username wasn't found
        {
            Console.WriteLine();
            Console.WriteLine("What is your name?");
            string? characterName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(characterName))
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            player = new MainCharacter(characterName, 100);
            DatabaseManager.SaveProgress(player.Name, player.LifeLevel, 0); // Initial save for new game
            Console.WriteLine($"Welcome {player.Name}, ready to brave the mystic woods? Your life level is {player.LifeLevel}.");
            Console.WriteLine("Your player has been created and an auto save has been performed. Click 's' when you want to save during the game.");
        }

        Console.WriteLine("Walk using keyboard arrows. \nPress 's' to save and 'q' to quit.");
        Console.WriteLine();

        bool isRunning = true;
        while (isRunning)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // The 'true' parameter prevents the key from being displayed.
            string direction = ""; // Initialize an empty string to store the direction.

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
                DatabaseManager.SaveProgress(player.Name, player.LifeLevel, distance);
                Console.WriteLine("Game saved successfully!");
            }
            else if (keyInfo.Key == ConsoleKey.Q)
            {
                Console.WriteLine("Exiting the game. Stay safe in the real world!");
                isRunning = false;
            }
        }
    }
}

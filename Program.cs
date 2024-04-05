using System;

class Program
{
    static void Main(string[] args)
    {
        DatabaseManager.InitializeDatabase();
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
            if (lifeLevel.HasValue && distanceLoaded.HasValue) // Check if values are not null
            {
                player = new MainCharacter(username, lifeLevel.Value);
                distance = distanceLoaded.Value;
                Console.Clear();
                Console.WriteLine($"Welcome back {player.Name}. \n Your lifeLevel is {player.LifeLevel}. \nYou have covered a distance of {distance} meters in the forest. Move carefully and stay safe!");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("User not found. Game has been terminated.");
                return; // Exit the program if no user or progress found
            }
        }

        if (player == null) // This means no restoration was done, either by choice or username not found
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
            Console.WriteLine();
            Console.Clear();
            Console.WriteLine($"Welcome {player.Name}. Your life level is {player.LifeLevel}.");
            Console.WriteLine();
            Console.WriteLine("Objective: Survive walking 500m in the forest!");
            Console.WriteLine();
            Console.WriteLine("INFO:\nAuto save has been performed. \nClick 's' to save during the game and 'q' to quit.\nRemember your user name to restore and continue later.");
        }
        Console.WriteLine();
        Console.WriteLine("Walk using keyboard arrows.");
        Console.WriteLine();

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
                Console.WriteLine("Exiting the game. Stay safe in the real world!");
                isRunning = false;
            }
        }
    }
}
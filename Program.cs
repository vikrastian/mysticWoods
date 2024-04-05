using System;

class Program
{
    static void Main(string[] args)
    {
        DatabaseManager.InitializeDatabase();

        Console.WriteLine("Do you want to restore a saved game? (yes/no)");
        string restoreChoice = Console.ReadLine()?.ToLower() ?? "";
        MainCharacter player = null; // Declare player variable here for wider scope
        int distance = 0; // Declare distance variable here for wider scope

        if (restoreChoice == "yes")
        {
            Console.WriteLine("Please enter your username:");
            string username = Console.ReadLine();

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
                Console.WriteLine($"Welcome back {player.Name}. Your lifeLevel is {player.LifeLevel} and you have covered a distance of {distance} meters in the forest. Move carefully and stay safe!");
                Console.WriteLine();

            }
            else
            {
                Console.WriteLine("No saved game found for this username, starting a new game.");
                Console.WriteLine();

            }
        }

        if (player == null) // This means either they chose not to restore, or the username wasn't found
        {
            Console.WriteLine("What is your name?");
            string characterName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(characterName))
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            player = new MainCharacter(characterName, 100);
            DatabaseManager.SaveProgress(player.Name, player.LifeLevel, 0); // Initial save for new game
            Console.WriteLine("Your player has been created and an auto save has been performed. \n Click 's' when you want to save during the game.");
            Console.WriteLine();

        }

        Console.WriteLine($"Welcome {player.Name} you have chosen to enter the mystic woods. \nYour life level is {player.LifeLevel}. \nFor your own safety, we only allow you to go 10m in each direction. \n \nLet the game begin.");
        Console.WriteLine("Use keyboard arrows to move your character in 10m increments to the forward, left, right. \nPress 'q' to quit.");
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
                Console.WriteLine();

            }

            if (keyInfo.Key == ConsoleKey.S)
            {
                DatabaseManager.SaveProgress(player.Name, player.LifeLevel, distance);
                Console.WriteLine("Game saved successfully!");
                Console.WriteLine();

            }
            else if (keyInfo.Key == ConsoleKey.Q)
            {
                Console.WriteLine("Chicken, did you get scared? The game has been exited.");
                Console.WriteLine();
                isRunning = false;
            }
            else if (string.IsNullOrEmpty(direction))
            {
                Console.WriteLine("Invalid choice of direction. Please select 'f', 'l', 'r', or 'q' to quit.");
                Console.WriteLine();

            }
        }
    }
}

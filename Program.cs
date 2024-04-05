using System;

class Program
{
    static void Main(string[] args)
    {
        bool isRunning = true;
        string intro = "You have reached the entrance of the Mystic Woods - do you dare to enter? (Yes/No)";
        foreach (char c in intro)
        {
            Console.Write(c);
            // Thread.Sleep(100); // Optional: Slow down text display.
        }
        Console.WriteLine(); // Ensure this is outside the foreach loop.

        string userInput = Console.ReadLine();
        if (userInput == null) return; // Early return if input is null.
        string lowerCaseInput = userInput.ToLower();

        if (lowerCaseInput == "yes")
        {
            Console.WriteLine("Are you willing to risk your body and soul to enter the forest?");
            userInput = Console.ReadLine()?.ToLower() ?? "";
            if (userInput == "yes")
            {
                Console.WriteLine("I am worried to welcome you to the Misterous Woods!");

                Console.WriteLine("Take a step into the woods and answer the following questions:");
            }
            else
            {
                Console.WriteLine("Smart choice, leave this forest and never return.");
                return;
            }
        }
        else if (lowerCaseInput == "no")
        {
            Console.WriteLine("You did the right choice, stay away from the mystic woods and stay safe.");
            return;
        }
        else
        {
            Console.WriteLine("Invalid character, are you shaking from fear? This forest is not for you.");
            return;
        }

        Console.WriteLine("What is your name?");
        string characterName = Console.ReadLine();
        if (characterName == null) return; // Early return if input is null.

        // Assuming MainCharacter is properly defined and following the updated Character class structure
        MainCharacter player = new MainCharacter(characterName, 100, 100);

        // Updated to use properties, assuming you've followed the previous advice on encapsulation
        Console.WriteLine($"Welcome {player.Name} you have chosen to enter the mystic woods. \nYour life level is {player.LifeLevel} and your armour is {player.Armour}\nFor your own safety we only allow you to go 10m in each direction. \n Let the game begin.");

        int distance = 0;
        Console.WriteLine("Use keyboard arrwos to move your character in 10m increments to the forward, left, right. Press 'q' to quit.");

        while (isRunning)
        {
            Console.Write("Choose your direction: ");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // The 'true' parameter prevents the key from being displayed.
            string direction = ""; // Initialize an empty string to store the direction.
            // string direction = Console.ReadLine()?.ToLower() ?? "";

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    isRunning = Navigation.NavigateAndCheckForEncounter(direction, player, ref distance);
                    Console.WriteLine($"Total distance moved: {distance} meters.");
                    break;
                case ConsoleKey.Q:
                    Console.WriteLine("Chicken, did you get scared? The game has been exited.");
                    isRunning
                     = false;
                    continue;
                default:
                    Console.WriteLine("Invalid choice of direction. Please select 'f', 'l', 'r', or 'q' to quit.");
                    break;
            }
        }
    }
}

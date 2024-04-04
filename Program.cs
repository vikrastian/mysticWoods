using System;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Net;
using System.Threading;

string intro = "You have reached the entrance of the Mystic Woods - do you dare to enter? (Yes/No)";
foreach (char c in intro)
{
    Console.Write(c);
    // Thread.Sleep(100);
}
Console.WriteLine();
string userInput = Console.ReadLine();
string lowerCaseInput = userInput.ToLower();

if (lowerCaseInput == "yes" || lowerCaseInput == "no")
{
    if (lowerCaseInput == "yes")
    {
        Console.WriteLine("Are you willing to risk your body and soul to enter the forest?");
        userInput = Console.ReadLine();
        lowerCaseInput = userInput.ToLower();
        if (lowerCaseInput == "yes")
        {
            Console.WriteLine("Take a step into the woods and answer the following questions:");
        }
        else
        {
            Console.WriteLine("Smart choice, leave this forest and never return.");
            return;
        }

    }
    else
    {
        Console.WriteLine("You did the right choice, stay away from the mystic woods and stay safe.");
        return;
    }
}
else
{
    Console.WriteLine("Invalid character, are you shaking from fear? This forest is not for you.");
    return;
}


Console.WriteLine("What is your name?");
string characterName = Console.ReadLine();

MainCharacter player = new MainCharacter(characterName, 100, 100);
publicbool isRunning = true;

Console.WriteLine($"Welcome {player.name} you have chosen to enter the mystic woods. \nYour life level is {player.lifeLevel} and your armour is {player.armour}\nFor your own safety we only allow you to go 10m in each direction. \n Let the game begin.");

int distance = 0;
Console.WriteLine("Use 'f' for forward, 'l' for left, 'r' for right to move your character in 10m increments Press 'q' to quit.");

while (isRunning)
{
    Console.Write("Choose your direction: ");
    string direction = Console.ReadLine().ToLower();

    switch (direction)
    {
        case "f":
        case "l":
        case "r":
            Navigation.NavigateAndCheckForEncounter(direction, player, ref distance);
            Console.WriteLine($"Total distance moved: {distance} meters.");
            break;
        case "q":
            Console.WriteLine("The game has been exited.");
            isRunning = false;
            break;
        default:
            Console.WriteLine("Invalid choice of direction. Please select 'f', 'l', 'r', or 'q' to quit.");
            break;
    }
}





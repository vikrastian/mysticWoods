using System;

public static class Navigation
{

    public static void NavigateAndCheckForEncounter(string direction, MainCharacter player, ref int distance)
    {
        distance += 10;
        // Generate a random number between 1 and 6
        Random random = new Random();
        int encounterChance = random.Next(1, 7); // Random.Next is inclusive for the min value and exclusive for the max value

        Console.WriteLine($"You move {direction}.");

        if (encounterChance == 6)
        {
            // Simulate an attack by an EvilCharacter
            int damage = random.Next(1, 101);
            player.lifeLevel -= damage;
            Console.WriteLine($"The misterous monster attacked you and diseappered without you being able to react! {player.lifeLevel}");

            // Check if player is defeated
            if (player.lifeLevel <= 0)
            {
                Console.WriteLine("You have been defeated by the mysterious monster!");
                isRunning = false;
            }

        }
        else
        {
            Console.WriteLine("You may continue if you dare, press q if you are to scared to continue.");
        }
    }

}
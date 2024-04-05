public static class Navigation
{
    public static bool NavigateAndCheckForEncounter(string direction, MainCharacter player, ref int distance)
    {
        distance += 10;
        Random random = new Random();
        int encounterChance = random.Next(1, 7);

        Console.WriteLine($"You move {direction}.");

        if (encounterChance == 6)
        {
            int damage = random.Next(1, 101);
            player.LifeLevel = Math.Max(0, player.LifeLevel - damage);

            Console.WriteLine($"A mysterious monster attacked you (-{damage}! Your life level is now {player.LifeLevel}");

            if (player.LifeLevel <= 0)
            {
                Console.WriteLine("You have been defeated by the mysterious monster!");
                return false; // Game should not continue
            }
        }
        else
        {
            Console.WriteLine("You may continue if you dare, press 'q' if you are too scared to continue.");
        }

        // Check for victory condition
        if (distance >= 500)
        {
            Console.WriteLine("Congratulations! You have survived 500 meters in the forest and won the game!");
            return false; // End the game
        }

        return true; // Game should continue
    }
}
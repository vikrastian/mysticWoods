// Base class for characters
public class Character
{
    public string Name { get; set; }
    public int LifeLevel { get; set; }


    public Character(string name, int lifeLevel)
    {
        Name = name;
        LifeLevel = lifeLevel;
    }

    public void Scream()
    {
        Console.WriteLine($"{Name} screaming");
    }
}

public class MainCharacter : Character
{
    public MainCharacter(string name, int lifeLevel) : base(name, lifeLevel)
    {

    }
}


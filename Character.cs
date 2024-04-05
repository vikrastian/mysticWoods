// Base class for characters
public class Character
{
    public string Name { get; set; }
    public int LifeLevel { get; set; }
    public int Armour { get; set; }

    public Character(string name, int lifeLevel, int armour)
    {
        Name = name;
        LifeLevel = lifeLevel;
        Armour = armour;
    }

    public void Scream()
    {
        Console.WriteLine($"{Name} screaming");
    }
}

public class MainCharacter : Character
{
    public MainCharacter(string name, int lifeLevel, int armour) : base(name, lifeLevel, armour)
    {

    }
}

public class EvilCharacter : Character
{
    public EvilCharacter(string name, int lifeLevel, int armour) : base(name, lifeLevel, armour)
    {

    }
}
// Base class for characters
public class Character
{
    public string name = "";
    public int lifeLevel = 100;
    public int armour = 0;

    public Character(string name, int lifeLevel, int armour)
    {
        this.name = name;
        this.lifeLevel = lifeLevel;
        this.armour = armour;
    }
    public void scream()
    {
        Console.WriteLine($"{name} screaming");
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
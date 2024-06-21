namespace LiteDB.StudioNew.Models;

public class Index
{
    public Index(string name, string expression, bool unique, int maxLevel)
    {
        Name = name;
        Expression = expression;
        Unique = unique;
        MaxLevel = maxLevel;
    }

    public string Name { get; }
    public string Expression { get; }
    public bool Unique { get; }
    public int MaxLevel { get; }
}
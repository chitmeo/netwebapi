namespace Dev.Core.Domain;

public class Setting
{
    public Setting()
    {
        Name = string.Empty;
        Value = string.Empty;
    }
    public Setting(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}

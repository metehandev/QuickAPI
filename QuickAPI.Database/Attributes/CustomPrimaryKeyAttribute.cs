namespace QuickAPI.Database.Attributes;

/// <summary>
/// Use this instead of default PrimaryKey attribute if Clustered Index should be set differently.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CustomPrimaryKeyAttribute : Attribute
{
    public string? Name { get; set; }
    public IReadOnlyList<string> PropertyNames { get; } = new List<string>();

    public CustomPrimaryKeyAttribute(string propertyName, params string[]? additionalPropertyNames)
    {
        ((List<string>)PropertyNames).Add(propertyName);
        if (additionalPropertyNames is not null)
        {
            ((List<string>)PropertyNames).AddRange(additionalPropertyNames);
        }
    }
}
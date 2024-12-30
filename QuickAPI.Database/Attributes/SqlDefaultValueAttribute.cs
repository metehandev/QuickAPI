namespace QuickAPI.Database.Attributes;

/// <summary>
/// Use this attribute if you want to mark a property with a default SQL value.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SqlDefaultValueAttribute : Attribute
{
    public SqlDefaultValueAttribute(string value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Default value to be used.
    /// </summary>
    public string Value { get; set; }
}
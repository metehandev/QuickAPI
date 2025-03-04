namespace MY.QuickAPI.Database.Attributes;

/// <summary>
/// Defines a custom primary key for a database table when the default primary key configuration needs to be modified.
/// This is particularly useful when you need to specify a different clustered index than the primary key.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SqlPrimaryKeyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the custom name for the primary key constraint.
    /// If not specified, a default name will be generated based on the table and column names.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the list of property names that form the primary key.
    /// These properties will be used to create a composite primary key if multiple properties are specified.
    /// </summary>
    public IReadOnlyList<string> PropertyNames { get; } = new List<string>();

    /// <summary>
    /// Initializes a new instance of the SqlPrimaryKeyAttribute with the specified property names.
    /// </summary>
    /// <param name="propertyName">The primary property name to include in the primary key.</param>
    /// <param name="additionalPropertyNames">Optional additional property names to create a composite primary key.</param>
    public SqlPrimaryKeyAttribute(string propertyName, params string[]? additionalPropertyNames)
    {
        ((List<string>)PropertyNames).Add(propertyName);
        if (additionalPropertyNames is not null)
        {
            ((List<string>)PropertyNames).AddRange(additionalPropertyNames);
        }
    }
}
namespace MY.QuickAPI.Database.Attributes;

/// <summary>
/// Specifies a default SQL value for a property when the database table is created or modified.
/// This attribute allows setting default constraints on database columns.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SqlDefaultValueAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the SqlDefaultValueAttribute with the specified default value.
    /// </summary>
    /// <param name="value">The SQL expression or literal value to use as the default value for the column.</param>
    public SqlDefaultValueAttribute(string value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Gets or sets the SQL default value expression or literal value.
    /// This value will be used as the default constraint for the database column.
    /// Examples include: 'GETDATE()', '0', 'NEWID()', etc.
    /// </summary>
    public string Value { get; set; }
}
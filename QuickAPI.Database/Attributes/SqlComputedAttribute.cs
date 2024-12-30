namespace QuickAPI.Database.Attributes;

/// <summary>
/// Defines a computed column in SQL Server that is derived from other columns or expressions.
/// The computed value is determined by the specified computation expression and can optionally be persisted.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SqlComputedAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the SQL expression used to compute the column value.
    /// This can be any valid SQL expression that references other columns in the same table.
    /// Example: '[FirstName] + ' ' + [LastName]' for a computed full name column.
    /// </summary>
    public string ComputeFunction { get; set; }
    
    /// <summary>
    /// Gets or sets whether the computed column should be physically stored in the table.
    /// When true, the computed value is persisted and stored physically, improving query performance
    /// at the cost of additional storage space and maintenance overhead during data modifications.
    /// </summary>
    public bool Stored { get; set; }

    /// <summary>
    /// Initializes a new instance of the SqlComputedAttribute with the specified computation expression.
    /// </summary>
    /// <param name="computeFunction">The SQL expression that defines how the column value should be computed.</param>
    public SqlComputedAttribute(string computeFunction)
    {
        ComputeFunction = computeFunction;
    }
}
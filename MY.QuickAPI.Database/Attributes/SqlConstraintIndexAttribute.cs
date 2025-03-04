namespace MY.QuickAPI.Database.Attributes;

/// <summary>
/// Attribute to define a constraint index on a database table class.
/// This attribute can be applied multiple times to create different indexes on the same table.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SqlConstraintIndexAttribute : Attribute
{
    /// <summary>
    /// Gets or sets whether this is a clustered index.
    /// A clustered index determines the physical order of data in a table.
    /// Only one clustered index can exist per table. By default, the primary key is the clustered index.
    /// To change the clustered index, use the CustomPrimaryKey attribute.
    /// </summary>
    public bool IsClustered { get; set; }
    
    /// <summary>
    /// Gets or sets the custom name for the index.
    /// If not specified, the name will be automatically generated as "{prefix}_{tableName}_{columnNames}",
    /// where prefix is either "IX" for non-unique indexes or "UQ" for unique indexes.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is a unique index.
    /// When true, the index enforces uniqueness of the indexed columns and changes the default name prefix from "IX" to "UQ".
    /// This ensures no duplicate values can exist in the indexed columns.
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// Gets or sets the additional non-key columns to include in the index.
    /// Including related columns can improve query performance by avoiding additional table lookups,
    /// as the included columns are stored at the leaf level of the index.
    /// </summary>
    public string[] IncludeProperties { get; set; } = [];

    /// <summary>
    /// Gets the list of property names that form the index key.
    /// These properties define the columns that will be indexed in the database table.
    /// </summary>
    public IReadOnlyList<string> PropertyNames { get; } = new List<string>();

    /// <summary>
    /// Initializes a new instance of the SqlConstraintIndexAttribute with the specified property names.
    /// </summary>
    /// <param name="propertyName">The primary property name to include in the index.</param>
    /// <param name="additionalPropertyNames">Optional additional property names to include in the composite index.</param>
    public SqlConstraintIndexAttribute(string propertyName, params string[]? additionalPropertyNames)
    {
        ((List<string>)PropertyNames).Add(propertyName);
        if (additionalPropertyNames is not null)
        {
            ((List<string>)PropertyNames).AddRange(additionalPropertyNames);
        }
    }
}
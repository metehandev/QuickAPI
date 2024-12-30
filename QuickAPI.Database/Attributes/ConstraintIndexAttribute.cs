namespace QuickAPI.Database.Attributes;

/// <summary>
/// Attribute to set Constaint Index to a class. 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ConstraintIndexAttribute : Attribute
{
    /// <summary>
    /// There can only be one Clustered index per table. PrimaryKey is default Clustered index. If clustered index
    /// should be changed, set table's primary key using CustomPrimaryKey attribute.
    /// </summary>
    public bool IsClustered { get; set; }
    
    /// <summary>
    /// Change this property to set the index's name. If not set, it will be created as $"{prefix}_{tableName}_{columnNames}"
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Set to true if the index should be a Unique index. Also changes default name prefix to UQ from IX. 
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// IncludeProperties is used to include related entities in the index.
    /// </summary>
    public string[] IncludeProperties { get; set; } = [];

    public IReadOnlyList<string> PropertyNames { get; } = new List<string>();
    public ConstraintIndexAttribute(string propertyName, params string[]? additionalPropertyNames)
    {
        ((List<string>)PropertyNames).Add(propertyName);
        if (additionalPropertyNames is not null)
        {
            ((List<string>)PropertyNames).AddRange(additionalPropertyNames);
        }
    }

}
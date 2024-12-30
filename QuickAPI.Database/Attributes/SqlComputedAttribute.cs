namespace QuickAPI.Database.Attributes;

/// <summary>
/// Use this attribute if you want to mark a property as Sql Computed. 
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SqlComputedAttribute : Attribute
{
    /// <summary>
    /// Set SQL Compute function here
    /// </summary>
    public string ComputeFunction { get; set; }
    
    /// <summary>
    /// Set to true if you want this SQL Computed field to be Stored aswell.
    /// </summary>
    public bool Stored { get; set; }

    public SqlComputedAttribute(string computeFunction)
    {
        ComputeFunction = computeFunction;
    }
}
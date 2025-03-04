namespace QuickAPI.Attributes;

/// <summary>
/// Order attribute for the Definition models to register into DI.
/// </summary>
/// <param name="order"></param>
[AttributeUsage(AttributeTargets.Class)]
public class DefinitionOrderAttribute(int order = int.MaxValue) : Attribute
{
    /// <summary>
    /// Order value
    /// </summary>
    public int Order { get; } = order;
}

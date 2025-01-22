namespace QuickAPI.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DefinitionOrderAttribute(int order = int.MaxValue) : Attribute
{
    public int Order { get; } = order;
}

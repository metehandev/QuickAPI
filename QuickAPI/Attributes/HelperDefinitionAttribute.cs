using QuickAPI.Core;

namespace QuickAPI.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class HelperDefinitionAttribute : Attribute
{
    public DependencyInjectionType DependencyInjectionType { get; set; } 
}
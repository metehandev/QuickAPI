using MY.QuickAPI.Core;

namespace MY.QuickAPI.Attributes;

/// <summary>
/// Attribute for setting Helper definition's Dependenjy Injection lifecycle
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class HelperDefinitionAttribute : Attribute
{
    /// <summary>
    /// Attribute for setting Helper definition's Dependenjy Injection lifecycle
    /// </summary>
    public DependencyInjectionType DependencyInjectionType { get; set; } 
}
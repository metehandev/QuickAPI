namespace QuickAPI.Core;


/// <summary>
/// Dependency Injection lifecycle type for Helper classes
/// </summary>
public enum DependencyInjectionType
{
    /// <summary>
    /// AddSingleton
    /// </summary>
    Singleton = 0,
    
    /// <summary>
    /// AddScoped
    /// </summary>
    Scoped = 1,
    
    /// <summary>
    /// AddTransient
    /// </summary>
    Transient = 2
}
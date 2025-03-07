using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MY.QuickAPI.Attributes;
using MY.QuickAPI.Core;
using MY.QuickAPI.Core.BaseConcretes;
using MY.QuickAPI.Database.Attributes;
using MY.QuickAPI.Database.DataModels;
using MY.QuickAPI.Helpers;
using MY.QuickAPI.Settings;

namespace MY.QuickAPI.Extensions;

/// <summary>
/// Entry point for MY.QuickAPI extensions
/// </summary>
public static class QuickApiExtensions
{
    private static void MapConfigurationSettings(this IServiceCollection services, params Type[] externalTypes)
    {
        externalTypes
            .SelectMany(m => m.Assembly.DefinedTypes)
            .Where(m => m.IsAssignableTo(typeof(ISettings))
                        && m is { IsInterface: false, IsAbstract: false })
            .Execute(services.RegisterSettings);
    }

    private static void MapHelpers(this IServiceCollection services, params Type[] externalTypes)
    {
        externalTypes
            .SelectMany(m => m.Assembly.DefinedTypes)
            .Where(m => m.IsAssignableTo(typeof(IHelper))
                        && m is { IsInterface: false, IsAbstract: false })
            .Execute(services.RegisterHelper);
    }


    private static void RegisterHelper(this IServiceCollection services, Type type)
    {
        var helperDefinition = type.GetCustomAttribute<HelperDefinitionAttribute>();
        if (helperDefinition is null)
        {
            services.AddSingleton(type);
            return;
        }

        var interfaces = type.GetInterfaces();
        var serviceType = interfaces.FirstOrDefault(m => m.IsAssignableTo(typeof(IHelper)) && m != typeof(IHelper));

        if (serviceType is not null)
        {
            switch (helperDefinition.DependencyInjectionType)
            {
                case DependencyInjectionType.Singleton:
                    services.AddSingleton(serviceType, type);
                    break;
                case DependencyInjectionType.Scoped:
                    services.AddScoped(serviceType, type);
                    break;
                case DependencyInjectionType.Transient:
                    services.AddTransient(serviceType, type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            return;
        }

        switch (helperDefinition.DependencyInjectionType)
        {
            case DependencyInjectionType.Singleton:
                services.AddSingleton(type);
                break;
            case DependencyInjectionType.Scoped:
                services.AddScoped(type);
                break;
            case DependencyInjectionType.Transient:
                services.AddTransient(type);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }

    private static void RegisterSettings(this IServiceCollection services, Type type)
    {
        const string settingsKey = "Settings";
        if (!type.Name.EndsWith(settingsKey))
        {
            throw new NotSupportedException(
                $"ISettings implementation's name must end with 'Settings'. {type.Name} is not applicable!");
        }

        var settingsIndex = type.Name.IndexOf(settingsKey, StringComparison.Ordinal);
        var sectionName = type.Name.Remove(settingsIndex, settingsKey.Length);

        var extensionsType = typeof(OptionsConfigurationServiceCollectionExtensions);
        var configureMethod = extensionsType.GetMethod(
            nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
            BindingFlags.Static | BindingFlags.Public,
            [typeof(IServiceCollection), typeof(IConfiguration)]);
        if (configureMethod is null)
        {
            return;
        }

        var scope = services.BuildServiceProvider().CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var configSection = config.GetSection(sectionName);

        var method = configureMethod.MakeGenericMethod(type);
        method.Invoke(null, [services, configSection]);
    }

    /// <summary>
    /// Register QuickApi layers on DI
    /// </summary>
    /// <param name="services">Services collection to inject DI items into</param>
    /// <param name="types">Type markers for detecting the QuickApi attribute models in an Assembly</param>
    public static void AddQuickApi(this IServiceCollection services,
        params Type[] types)
    {
        services.MapConfigurationSettings(typeof(BaseModel), typeof(ISettings));
        services.MapConfigurationSettings(types);

        services.MapHelpers(typeof(BaseModel), typeof(IHelper));
        services.MapHelpers(types);

        var endpointDefinitions = new List<IDefinition>();

        var externalDefinitionTypes = types
            .Select(m => m.Assembly)
            .Except([typeof(QuickApiExtensions).Assembly])
            .SelectMany(m => m.DefinedTypes)
            .Where(m => m.IsAssignableTo(typeof(IDefinition)) && !m.IsAssignableTo(typeof(IEndpointDefinition))
                                                              && m is
                                                              {
                                                                  IsInterface: false, IsAbstract: false,
                                                                  IsGenericType: false
                                                              })
            .OrderBy(m => m.GetCustomAttribute<DefinitionOrderAttribute>()?.Order ?? int.MaxValue)
            .ToList();

        var internalDefinitionTypes = typeof(IDefinition)
            .Assembly
            .DefinedTypes
            .Where(m => m.IsAssignableTo(typeof(IDefinition)) && !m.IsAssignableTo(typeof(IEndpointDefinition))
                                                              && m is
                                                              {
                                                                  IsInterface: false, IsAbstract: false,
                                                                  IsGenericType: false
                                                              })
            .OrderBy(m => m.GetCustomAttribute<DefinitionOrderAttribute>()?.Order ?? int.MaxValue)
            .ToList();

        foreach (var type in internalDefinitionTypes)
        {
            var provider = services.BuildServiceProvider();
            var internalEndpoint = (IDefinition)ActivatorUtilities.CreateInstance(provider, type);
            var inheritedEndpoint = externalDefinitionTypes.FirstOrDefault(m => type.IsAssignableFrom(m));
            if (inheritedEndpoint is not null)
            {
                continue;
            }

            internalEndpoint.DefineServices(services);
            endpointDefinitions.Add(internalEndpoint);
        }

        foreach (var type in externalDefinitionTypes)
        {
            var provider = services.BuildServiceProvider();
            var externalEndpoint = (IDefinition)ActivatorUtilities.CreateInstance(provider, type);
            externalEndpoint.DefineServices(services);
            endpointDefinitions.Add(externalEndpoint);
        }
        
        var baseModelTypes = types
            .SelectMany(m => m.Assembly.DefinedTypes)
            .Where(m => m.IsAssignableTo(typeof(BaseModel))
                        && m is { IsInterface: false, IsAbstract: false }
                        && m.GetCustomAttribute<EndpointDefinitionAttribute>() != null);

        baseModelTypes.Execute(endpointBaseModel =>
        {
            var endpointDefinition = services.RegisterBaseEndpointDefinition(endpointBaseModel);
            if (endpointDefinition is null)
            {
                return;
            }

            endpointDefinitions.Add(endpointDefinition);
        });

        var externalEndpointTypes = types
            .Select(m => m.Assembly)
            .Except([typeof(QuickApiExtensions).Assembly])
            .SelectMany(m => m.DefinedTypes)
            .Where(m => m.IsAssignableTo(typeof(IEndpointDefinition))
                        && m is { IsInterface: false, IsAbstract: false, IsGenericType: false })
            .OrderBy(m => m.GetCustomAttribute<DefinitionOrderAttribute>()?.Order ?? int.MaxValue)
            .ToList();

        var internalEndpointTypes = typeof(IEndpointDefinition)
            .Assembly
            .DefinedTypes
            .Where(m => m.IsAssignableTo(typeof(IEndpointDefinition))
                        && m is { IsInterface: false, IsAbstract: false, IsGenericType: false })
            .OrderBy(m => m.GetCustomAttribute<DefinitionOrderAttribute>()?.Order ?? int.MaxValue)
            .ToList();

        foreach (var type in internalEndpointTypes)
        {
            var provider = services.BuildServiceProvider();
            var internalEndpoint = (IEndpointDefinition)ActivatorUtilities.CreateInstance(provider, type);
            var inheritedEndpoint = externalEndpointTypes.FirstOrDefault(m => type.IsAssignableFrom(m));
            if (inheritedEndpoint is not null)
            {
                continue;
            }

            internalEndpoint.DefineServices(services);
            endpointDefinitions.Add(internalEndpoint);
        }

        foreach (var type in externalEndpointTypes)
        {
            var provider = services.BuildServiceProvider();
            var externalEndpoint = (IEndpointDefinition)ActivatorUtilities.CreateInstance(provider, type);
            externalEndpoint.DefineServices(services);
            endpointDefinitions.Add(externalEndpoint);
        }


        services.AddSingleton<IReadOnlyCollection<IDefinition>>(endpointDefinitions);
    }

    private static IDefinition? RegisterBaseEndpointDefinition(this IServiceCollection services,
        Type endpointBaseModel)
    {
        var endpointDefinitionAttribute = endpointBaseModel.GetCustomAttribute<EndpointDefinitionAttribute>()!;
        if (!endpointDefinitionAttribute.AutomaticEndpointCreation)
        {
            return null;
        }

        // Determine which endpoint definition type to use based on DtoType property
        Type endpointDefinitionType;
        if (endpointDefinitionAttribute.DtoType is null)
        {
            // Use regular BaseEndpointDefinition<T> if no DTO type is specified
            var baseEndpointDefinitionType = typeof(BaseEndpointDefinition<>);
            endpointDefinitionType = baseEndpointDefinitionType.MakeGenericType(endpointBaseModel);
        }
        else
        {
            // Use BaseDtoEndpointDefinition<T, TDto> if DTO type is specified
            var baseDtoEndpointDefinitionType = typeof(BaseDtoEndpointDefinition<,>);
            endpointDefinitionType =
                baseDtoEndpointDefinitionType.MakeGenericType(endpointBaseModel, endpointDefinitionAttribute.DtoType);

            // Register the mapper for this model/DTO pair if it doesn't exist
            RegisterMapperIfNeeded(services, endpointBaseModel, endpointDefinitionAttribute.DtoType);
        }

        var provider = services.BuildServiceProvider();
        if (ActivatorUtilities.CreateInstance(provider, endpointDefinitionType) is not IEndpointDefinition
            endpointDefinition)
        {
            return null;
        }

        endpointDefinition.CrudOperation = endpointDefinitionAttribute.CrudOperation;
        endpointDefinition.CommonRole = endpointDefinitionAttribute.CommonRole;
        endpointDefinition.MethodRoles = new Dictionary<HttpMethod, string>()
        {
            { HttpMethod.Get, endpointDefinitionAttribute.GetRole },
            { HttpMethod.Post, endpointDefinitionAttribute.PostRole },
            { HttpMethod.Put, endpointDefinitionAttribute.PutRole },
            { HttpMethod.Delete, endpointDefinitionAttribute.DeleteRole },
        };

        endpointDefinition.MethodAllowAnonymouses = new Dictionary<HttpMethod, bool>()
        {
            { HttpMethod.Get, endpointDefinitionAttribute.AllowAnonymousGet },
            { HttpMethod.Post, endpointDefinitionAttribute.AllowAnonymousPost },
            { HttpMethod.Put, endpointDefinitionAttribute.AllowAnonymousPut },
            { HttpMethod.Delete, endpointDefinitionAttribute.AllowAnonymousDelete },
        };

        endpointDefinition.RequireAuthorization = endpointDefinitionAttribute.RequireAuthorization;
        endpointDefinition.IncludeFields = endpointDefinitionAttribute.IncludeFields;
        endpointDefinition.DefineServices(services);
        return endpointDefinition;
    }

    private static void RegisterMapperIfNeeded(IServiceCollection services, Type modelType, Type dtoType)
    {
        // Check if a custom mapper is already registered for this model/DTO pair
        var mapperInterfaceType = typeof(IModelDtoMapper<,>).MakeGenericType(modelType, dtoType);

        // Look for any custom mapper implementations in the services collection
        var provider = services.BuildServiceProvider();
        var existingMapper = provider.GetService(mapperInterfaceType);

        if (existingMapper == null)
        {
            // No custom mapper found, so register an open type for the SimpleModelDtoMapper
            services.AddTransient(mapperInterfaceType, _ => throw new InvalidOperationException(
                $"No mapper found for model type {modelType.Name} and DTO type {dtoType.Name}. " +
                $"Please register a concrete implementation of IModelDtoMapper<{modelType.Name}, {dtoType.Name}>."));
        }
    }

    /// <summary>
    /// QuickApi use on built application
    /// </summary>
    /// <param name="app"></param>
    public static void UseQuickApi(this WebApplication app)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IDefinition>>();
        foreach (var endpointDefinition in definitions)
        {
            endpointDefinition.Define(app);
        }
    }
}
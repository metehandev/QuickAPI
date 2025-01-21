using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickAPI.Attributes;
using QuickAPI.Core;
using QuickAPI.Core.BaseConcretes;
using QuickAPI.Database.Attributes;
using QuickAPI.Database.DataModels;
using QuickAPI.Helpers.Core;
using QuickAPI.Settings;

namespace QuickAPI.Extensions;

public static class QuickApiExtensions
{
    private static void MapConfigurationSettings(this IServiceCollection services, params Type[] externalTypes)
    {
        externalTypes
            .SelectMany(m => m.Assembly.ExportedTypes)
            .Where(m => m.IsAssignableTo(typeof(ISettings))
                        && m is { IsInterface: false, IsAbstract: false })
            .Execute(services.RegisterSettings);
    }

    private static void MapHelpers(this IServiceCollection services, params Type[] externalTypes)
    {
        externalTypes
            .SelectMany(m => m.Assembly.ExportedTypes)
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

    public static void AddQuickApi(this IServiceCollection services, bool automaticEndpointCreation = true,
        params Type[] types)
    {
        services.MapHelpers(typeof(BaseModel), typeof(IHelper));
        services.MapHelpers(types);

        services.MapConfigurationSettings(typeof(BaseModel), typeof(ISettings));
        services.MapConfigurationSettings(types);

        var endpointDefinitions = new List<IDefinition>();

        


        var externalDefinitionTypes = types
            .Select(m => m.Assembly)
            .Except([typeof(QuickApiExtensions).Assembly])
            .SelectMany(m => m.ExportedTypes)
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
            .ExportedTypes
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
        
        if (automaticEndpointCreation)
        {
            var baseModelTypes = types
                .SelectMany(m => m.Assembly.ExportedTypes)
                .Where(m => m.IsAssignableTo(typeof(BaseModel))
                            && m is { IsInterface: false, IsAbstract: false }
                            && m.GetCustomAttribute<EndpointDefinitionAttribute>() != null);

            baseModelTypes.Execute(endpointBaseModel => services.RegisterBaseEndpointDefinition(
                typeof(BaseEndpointDefinition<>), endpointBaseModel, endpointDefinitions));
        }

        var externalEndpointTypes = types
            .Select(m => m.Assembly)
            .Except([typeof(QuickApiExtensions).Assembly])
            .SelectMany(m => m.ExportedTypes)
            .Where(m => m.IsAssignableTo(typeof(IEndpointDefinition))
                        && m is { IsInterface: false, IsAbstract: false, IsGenericType: false })
            .OrderBy(m => m.GetCustomAttribute<DefinitionOrderAttribute>()?.Order ?? int.MaxValue)
            .ToList();

        var internalEndpointTypes = typeof(IEndpointDefinition)
            .Assembly
            .ExportedTypes
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

    private static void RegisterBaseEndpointDefinition(this IServiceCollection services,
        Type baseEndpointDefinitionType, Type endpointBaseModel, List<IDefinition> endpointDefinitions)
    {
        var endpointDefinitionAttribute = endpointBaseModel.GetCustomAttribute<EndpointDefinitionAttribute>()!;
        if (!endpointDefinitionAttribute.AutomaticEndpointCreation)
        {
            return;
        }

        var endpointDefinitionType = baseEndpointDefinitionType.MakeGenericType(endpointBaseModel);
        // var endpointDefinitionType = endpointDefinitionAttribute.DtoType is null
        //     ? baseEndpointDefinitionType.MakeGenericType(endpointBaseModel)
        //     : baseEndpointDefinitionType.MakeGenericType(endpointBaseModel, endpointDefinitionAttribute.DtoType);

        var provider = services.BuildServiceProvider();
        if (ActivatorUtilities.CreateInstance(provider, endpointDefinitionType) is not IEndpointDefinition endpointDefinition)
        {
            return;
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
        endpointDefinition.DefineServices(services);
        endpointDefinitions.Add(endpointDefinition);
    }

    public static void UseQuickApi(this WebApplication app)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IDefinition>>();
        foreach (var endpointDefinition in definitions)
        {
            endpointDefinition.Define(app);
        }
    }
}
using System.Security.Claims;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using QuickAPI.Core;
using QuickAPI.Core.BaseConcretes;
using QuickAPI.Database.Core;
using QuickAPI.Database.Data;
using QuickAPI.Example.DataModels;
using QuickAPI.Example.Dtos;
using QuickAPI.Example.Mappers;

namespace QuickAPI.Example.Endpoints;

/// <summary>
/// Example 2: Custom endpoint definition class that inherits from BaseDtoEndpointDefinition
/// This approach gives you more control over the endpoint behavior and events
/// </summary>
public class CategoryEndpoint : BaseDtoEndpointDefinition<Category, CategoryDto>
{
    // Configure CRUD operations
    public override CrudOperation CrudOperation { get; set; } = CrudOperation.All;

    // Configure authorization
    public override bool RequireAuthorization { get; set; } = true;
    public override string CommonRole { get; set; } = nameof(UserRole.Admin);

    public override Dictionary<HttpMethod, bool> MethodAllowAnonymouses { get; set; } = new()
    {
        { HttpMethod.Get, true }
    };

    public CategoryEndpoint(
        BaseContext context,
        ILogger<CategoryEndpoint> logger,
        IModelDtoMapper<Category, CategoryDto> mapper) : base(context, logger, mapper)
    {
        // Set up event hooks
        OnBeforeGetMany = BeforeGetMany;
        OnAfterGetMany = AfterGetMany;

        // Additional customization for specific events can be added here
    }

    private async Task AfterGetMany(LoadResult result)
    {
        if (result.data != null)
        {
            // Convert to object array first, then get the length
            var dataArray = result.data as object[];
            int count = dataArray?.Length ?? 0;
            Logger.LogInformation("Returned {Count} categories", count);
        }
        else
        {
            Logger.LogInformation("Returned empty result");
        }

        await Task.CompletedTask;
    }

    private async Task BeforeGetMany(ClaimsPrincipal claimsPrincipal,
        DataSourceLoadOptionsBase? dataSourceLoadOptionsBase)
    {
        Logger.LogInformation("Custom before-hook for Get Many Categories executed");
        await Task.CompletedTask;
    }

    public override void DefineServices(IServiceCollection services)
    {
        // Register the CategoryMapper
    }

    // Optional: Override methods to add custom behavior
    protected override async Task<IResult> GetManyAsync(
        ClaimsPrincipal claimsPrincipal,
        BindableDataSourceLoadOptions options)
    {
        // You can add custom behavior here, or just call the base implementation
        Logger.LogInformation("Custom GetManyAsync method called for CategoryEndpoint");

        // Calculate product counts for each category before returning
        var result = await base.GetManyAsync(claimsPrincipal, options);

        return result;
    }
}
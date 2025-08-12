using System.Security.Claims;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using MY.QuickAPI.Core;
using MY.QuickAPI.Core.BaseConcretes;
using MY.QuickAPI.Database.Core;
using MY.QuickAPI.Database.Data;
using MY.QuickAPI.Example.DataModels;
using MY.QuickAPI.Example.Dtos;

namespace MY.QuickAPI.Example.Endpoints;

/// <summary>
/// Example 2: Custom endpoint definition class that inherits from BaseDtoEndpointDefinition
/// This approach gives you more control over the endpoint behavior and events
/// </summary>
public class CategoryEndpoint : BaseDtoEndpointDefinition<Category, CategoryDto>
{


    public CategoryEndpoint(
        ILogger<CategoryEndpoint> logger,
        IModelDtoMapper<Category, CategoryDto> mapper) : base(logger, mapper)
    {
        // Set up event hooks
        OnBeforeGetMany = BeforeGetMany;
        OnAfterGetMany = AfterGetMany;
        CrudOperation = CrudOperation.All;
        RequireAuthorization = true;
        CommonRole = nameof(UserRole.Admin);
        MethodAllowAnonymouses.Add(HttpMethod.Get, true);
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


    protected override async Task<IResult> GetManyAsync(BaseContext context, ClaimsPrincipal claimsPrincipal, BindableDataSourceLoadOptions options)
    {
        Logger.LogInformation("Custom GetManyAsync method called for CategoryEndpoint");

        // Calculate product counts for each category before returning
        var result = await base.GetManyAsync(context, claimsPrincipal, options);

        return result;
    }
}
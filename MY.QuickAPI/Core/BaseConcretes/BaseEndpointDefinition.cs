using System.Security.Claims;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MY.QuickAPI.Database.Core;
using MY.QuickAPI.Database.Data;
using MY.QuickAPI.Database.DataModels;
using static Microsoft.AspNetCore.Http.Results;

namespace MY.QuickAPI.Core.BaseConcretes;

/// <summary>
/// Base endpoint definition class for endpoints that use Models itself for input/output
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseEndpointDefinition<T> : EndPointDefinitionBase, IEndpointDefinition
    where T : BaseModel
{
    /// <summary>
    /// Injected Logger
    /// </summary>
    protected readonly ILogger<BaseEndpointDefinition<T>> Logger;

    // Before event handlers
    /// <summary>
    /// Assign method to run sync events on before GET method
    /// </summary>
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeGet;

    /// <summary>
    /// Assign method to run sync events on before GET Many method
    /// </summary>
    protected Func<ClaimsPrincipal, DataSourceLoadOptionsBase?, Task>? OnBeforeGetMany;

    /// <summary>
    /// Assign method to run sync events on before POST method
    /// </summary>
    protected Func<ClaimsPrincipal, T, Task>? OnBeforePost;

    /// <summary>
    /// Assign method to run sync events on before PUT method
    /// </summary>
    protected Func<ClaimsPrincipal, T, Task>? OnBeforePut;

    /// <summary>
    /// Assign method to run sync events on before DELETE method
    /// </summary>
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeDelete;

    // Async Before event handlers
    /// <summary>
    /// Assign method to run async events on before GET method
    /// </summary>
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeGetAsync;

    /// <summary>
    /// Assign method to run async events on before GET Many method
    /// </summary>
    protected Func<ClaimsPrincipal, DataSourceLoadOptionsBase?, Task>? OnBeforeGetManyAsync;

    /// <summary>
    /// Assign method to run async events on before POST method
    /// </summary>
    protected Func<ClaimsPrincipal, T, Task>? OnBeforePostAsync;

    /// <summary>
    /// Assign method to run async events on before PUT method
    /// </summary>
    protected Func<ClaimsPrincipal, T, Task>? OnBeforePutAsync;

    /// <summary>
    /// Assign method to run async events on before DELETE method
    /// </summary>
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeDeleteAsync;

    // After event handlers

    /// <summary>
    /// Assign method to run sync events on after GET method
    /// </summary>
    protected Func<T, Task>? OnAfterGet;

    /// <summary>
    /// Assign method to run sync events on after GET Many method
    /// </summary>
    protected Func<LoadResult, Task>? OnAfterGetMany;

    /// <summary>
    /// Assign method to run sync events on after POST method
    /// </summary>
    protected Func<T, Task>? OnAfterPost;

    /// <summary>
    /// Assign method to run sync events on after PUT method
    /// </summary>
    protected Func<T, Task>? OnAfterPut;

    /// <summary>
    /// Assign method to run sync events on after DELETE method
    /// </summary>
    protected Func<Task>? OnAfterDelete;

    // Async After event handlers
    /// <summary>
    /// Assign method to run async events on after GET method
    /// </summary>
    protected Func<T, Task>? OnAfterGetAsync;

    /// <summary>
    /// Assign method to run async events on after GET Many method
    /// </summary>
    protected Func<LoadResult, Task>? OnAfterGetManyAsync;

    /// <summary>
    /// Assign method to run async events on after POST method
    /// </summary>
    protected Func<T, Task>? OnAfterPostAsync;

    /// <summary>
    /// Assign method to run async events on after PUT method
    /// </summary>
    protected Func<T, Task>? OnAfterPutAsync;

    /// <summary>
    /// Assign method to run async events on after DELETE method
    /// </summary>
    protected Func<Task>? OnAfterDeleteAsync;

    /// <summary>
    /// Base constructor to inject Logger
    /// </summary>
    /// <param name="logger"></param>
    public BaseEndpointDefinition(ILogger<BaseEndpointDefinition<T>> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Define method to set REST Method mappings using minimal APIs 
    /// </summary>
    /// <param name="app"></param>
    public override void Define(WebApplication app)
    {
        var type = typeof(T);
        var typeName = type.Name;
        var groupName = typeName + "s";

        var getRouteHandlers = new List<RouteHandlerBuilder>();
        var postRouteHandlers = new List<RouteHandlerBuilder>();
        var putRouteHandlers = new List<RouteHandlerBuilder>();
        var deleteRouteHandlers = new List<RouteHandlerBuilder>();
        if (CrudOperation.HasFlag(CrudOperation.Get))
        {
            var get = app.MapGet($"/api/{typeName}", GetAsync)
                .Produces<T>()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.Get)} {typeName}")
                .WithName($"Get{typeName}");
            getRouteHandlers.Add(get);
        }

        if (CrudOperation.HasFlag(CrudOperation.GetMany))
        {
            var getMany = app.MapGet($"/api/{groupName}", GetManyAsync)
                .Produces<LoadResult>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.GetMany)} {typeName}")
                .WithName($"Get{groupName}");
            getRouteHandlers.Add(getMany);
        }

        if (CrudOperation.HasFlag(CrudOperation.Post))
        {
            var post = app.MapPost($"/api/{typeName}", AddAsync)
                .Produces<T>(StatusCodes.Status302Found)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.Post)} {typeName}")
                .WithName($"Post{typeName}");
            postRouteHandlers.Add(post);
        }

        if (CrudOperation.HasFlag(CrudOperation.Put))
        {
            var put = app.MapPut($"/api/{typeName}", UpdateAsync)
                .Produces<T>(StatusCodes.Status302Found)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.Put)} {typeName}")
                .WithName($"Put{typeName}");
            putRouteHandlers.Add(put);
        }

        if (CrudOperation.HasFlag(CrudOperation.Delete))
        {
            var delete = app.MapDelete($"/api/{typeName}", RemoveAsync)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.Delete)} {typeName}")
                .WithName($"Delete{typeName}");
            deleteRouteHandlers.Add(delete);
        }

        if (CrudOperation.HasFlag(CrudOperation.PostMany))
        {
            var postMany = app.MapPost($"/api/{groupName}", AddManyAsync)
                .Produces<IEnumerable<T>>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.PostMany)} {typeName}")
                .WithName($"Post{groupName}");
            postRouteHandlers.Add(postMany);
        }

        BindAuthorizationOptionsForVerb(HttpMethod.Get, getRouteHandlers.ToArray());
        BindAuthorizationOptionsForVerb(HttpMethod.Post, postRouteHandlers.ToArray());
        BindAuthorizationOptionsForVerb(HttpMethod.Put, putRouteHandlers.ToArray());
        BindAuthorizationOptionsForVerb(HttpMethod.Delete, deleteRouteHandlers.ToArray());
    }

    /// <summary>
    /// Overridable base method for POST Many method
    /// </summary>
    /// <param name="context">Scoped EF Core database context.</param>
    /// <param name="claimsPrincipal">Authenticated user making the request.</param>
    /// <param name="items">Entities to add in bulk.</param>
    /// <returns>HTTP result with created entities or error.</returns>
    protected virtual async Task<IResult> AddManyAsync(
        BaseContext context,
        ClaimsPrincipal claimsPrincipal,
        [FromBody] IEnumerable<T> items)
    {
        try
        {
            var list = items.ToList();
            await context.Set<T>().AddRangeAsync(list);
            await context.SaveChangesAsync();
            return Ok(list);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on AddManyAsync for {TypeName}: {ErrorMessage}", typeof(T).Name, ex.Message);
            return BadRequest($"An error occurred while adding multiple {typeof(T).Name} items: {ex.Message}");
        }
    }


    /// <summary>
    /// Register any necessary methods to DI 
    /// </summary>
    /// <param name="services"></param>
    public override void DefineServices(IServiceCollection services)
    {
    }

    /// <summary>
    /// Overridable base method for GET method
    /// </summary>
    /// <param name="context">Scoped EF Core database context.</param>
    /// <param name="claimsPrincipal">Authenticated user making the request.</param>
    /// <param name="id">Entity identifier.</param>
    /// <param name="includeFields">Navigation properties to include.</param>
    /// <returns>HTTP result with the entity or error.</returns>
    protected virtual async Task<IResult> GetAsync(
        BaseContext context,
        ClaimsPrincipal claimsPrincipal,
        Guid id,
        string[]? includeFields = null)
    {
        try
        {
            if (OnBeforeGet is not null)
            {
                await OnBeforeGet.Invoke(claimsPrincipal, id);
            }

            OnBeforeGetAsync?.Invoke(claimsPrincipal, id);

            Logger.LogInformation("Getting {TypeName} for id {Id}", typeof(T).Name, id);

            var dbSet = IncludeNavigations(context.Set<T>().AsNoTracking(), includeFields);
            var item = await dbSet.FirstOrDefaultAsync(m => m.Id == id);
            if (item is null)
            {
                Logger.LogWarning("No {TypeName} found for id {Id}", typeof(T).Name, id);
                return NotFound();
            }

            if (OnAfterGet is not null)
            {
                await OnAfterGet.Invoke(item);
            }

            OnAfterGetAsync?.Invoke(item);

            return Ok(item);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on GetAsync for {TypeName} id {Id}: {ErrorMessage}", typeof(T).Name, id, ex.Message);
            return BadRequest($"An error occurred while retrieving {typeof(T).Name} with id {id}: {ex.Message}");
        }
    }

    /// <summary>
    /// Overridable base method for GET Many method
    /// </summary>
    /// <param name="context">Scoped EF Core database context.</param>
    /// <param name="claimsPrincipal">Authenticated user making the request.</param>
    /// <param name="options">DevExtreme data loading options.</param>
    /// <returns>Paged/sorted data result or error.</returns>
    protected virtual async Task<IResult> GetManyAsync(
        BaseContext context,
        ClaimsPrincipal claimsPrincipal,
        BindableDataSourceLoadOptions options)
    {
        try
        {
            if (OnBeforeGetMany is not null)
            {
                await OnBeforeGetMany.Invoke(claimsPrincipal, options);
            }

            OnBeforeGetManyAsync?.Invoke(claimsPrincipal, options);

            Logger.LogInformation("Getting {TypeName} items", typeof(T).Name);

            var dbSet = IncludeNavigations(context.Set<T>().AsNoTracking(), options.IncludeFields);
            var result = await DataSourceLoader.LoadAsync(dbSet, options);

            if (OnAfterGetMany is not null)
            {
                await OnAfterGetMany.Invoke(result);
            }

            OnAfterGetManyAsync?.Invoke(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on GetManyAsync for {TypeName}: {ErrorMessage}", typeof(T).Name, ex.Message);
            return BadRequest($"An error occurred while retrieving list of {typeof(T).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Overridable base method for POST method
    /// </summary>
    /// <param name="context">Scoped EF Core database context.</param>
    /// <param name="claimsPrincipal">Authenticated user making the request.</param>
    /// <param name="item">Entity to create.</param>
    /// <returns>Created result with location or error.</returns>
    protected virtual async Task<IResult> AddAsync(
        BaseContext context,
        ClaimsPrincipal claimsPrincipal,
        T item)
    {
        try
        {
            var type = typeof(T);
            var typeName = type.Name;

            if (OnBeforePost is not null)
            {
                await OnBeforePost.Invoke(claimsPrincipal, item);
            }

            OnBeforePostAsync?.Invoke(claimsPrincipal, item);

            Logger.LogInformation("Adding new {TypeName}", typeName);

            await context.Set<T>().AddAsync(item);
            await context.SaveChangesAsync();

            if (OnAfterPost is not null)
            {
                await OnAfterPost.Invoke(item);
            }

            OnAfterPostAsync?.Invoke(item);

            return CreatedAtRoute($"Get{typeName}", new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on AddAsync for {TypeName}: {ErrorMessage}", typeof(T).Name, ex.Message);
            return BadRequest($"An error occurred while adding {typeof(T).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Overridable base method for PUT method
    /// </summary>
    /// <param name="context">Scoped EF Core database context.</param>
    /// <param name="claimsPrincipal">Authenticated user making the request.</param>
    /// <param name="item">Entity to update.</param>
    /// <returns>Updated result or error.</returns>
    protected virtual async Task<IResult> UpdateAsync(
        BaseContext context,
        ClaimsPrincipal claimsPrincipal,
        T item)
    {
        try
        {
            if (OnBeforePut is not null)
            {
                await OnBeforePut.Invoke(claimsPrincipal, item);
            }

            OnBeforePutAsync?.Invoke(claimsPrincipal, item);

            var type = typeof(T);
            var typeName = type.Name;
            Logger.LogInformation("Updating {TypeName}", typeName);

            var result = await context.Set<T>().FindAsync(item.Id);

            if (result is null)
            {
                Logger.LogWarning("No {TypeName} found to update for id {Id}", typeof(T).Name, item.Id);
                return NotFound();
            }

            context.Entry(result).CurrentValues.SetValues(item);
            await context.SaveChangesAsync();

            if (OnAfterPut is not null)
            {
                await OnAfterPut.Invoke(item);
            }

            OnAfterPutAsync?.Invoke(item);

            return CreatedAtRoute($"Get{typeName}", new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on UpdateAsync for {TypeName} id {Id}: {ErrorMessage}", typeof(T).Name, item.Id, ex.Message);
            return BadRequest($"An error occurred while updating {typeof(T).Name} with id {item.Id}: {ex.Message}");
        }
    }

    /// <summary>
    /// Overridable base method for DELETE method
    /// </summary>
    /// <param name="context">Scoped EF Core database context.</param>
    /// <param name="logger">Logger instance for diagnostics.</param>
    /// <param name="claimsPrincipal">Authenticated user making the request.</param>
    /// <param name="id">Entity identifier.</param>
    /// <returns>OK or error result.</returns>
    protected virtual async Task<IResult> RemoveAsync(
        BaseContext context,
        ILogger<BaseEndpointDefinition<T>> logger,
        ClaimsPrincipal claimsPrincipal,
        Guid id)
    {
        try
        {
            if (OnBeforeDelete is not null)
            {
                await OnBeforeDelete.Invoke(claimsPrincipal, id);
            }

            OnBeforeDeleteAsync?.Invoke(claimsPrincipal, id);

            logger.LogInformation("Deleting {TypeName} for id {Id}", typeof(T).Name, id);
            var result = await context.Set<T>().FindAsync(id);
            if (result is null)
            {
                logger.LogWarning("No {TypeName} found to delete for id {Id}", typeof(T).Name, id);
                return NotFound();
            }

            context.Set<T>().Remove(result);
            await context.SaveChangesAsync();

            OnAfterDeleteAsync?.Invoke();
            if (OnAfterDelete is not null)
            {
                await OnAfterDelete.Invoke();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error on RemoveAsync for {TypeName} id {Id}: {ErrorMessage}", typeof(T).Name, id, ex.Message);
            return BadRequest($"An error occurred while deleting {typeof(T).Name} with id {id}: {ex.Message}");
        }
    }

    private IQueryable<T> IncludeNavigations(IQueryable<T> dbSet, string[]? includeFields)
    {
        if (includeFields?.Length > 0)
        {
            foreach (var includeField in includeFields)
            {
                dbSet = dbSet.Include(includeField);
            }

            return dbSet;
        }

        if (IncludeFields.Length <= 0) 
            return dbSet;
        
        foreach (var includeField in IncludeFields)
        {
            dbSet = dbSet.Include(includeField);
        }

        return dbSet;

    }
}

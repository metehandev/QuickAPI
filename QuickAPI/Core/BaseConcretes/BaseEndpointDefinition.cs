using System.Security.Claims;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickAPI.Database.Core;
using QuickAPI.Database.Data;
using QuickAPI.Database.DataModels;
using static Microsoft.AspNetCore.Http.Results;

namespace QuickAPI.Core.BaseConcretes;

/// <summary>
/// Base endpoint definition class for endpoints that use Models itself for input/output
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseEndpointDefinition<T> : EndPointDefinitionBase, IEndpointDefinition
    where T : BaseModel
{
    /// <summary>
    /// Injected DbContext
    /// </summary>
    protected readonly BaseContext Context;
    
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
    /// Base constructor to inject Context and Logger
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public BaseEndpointDefinition(
        BaseContext context,
        ILogger<BaseEndpointDefinition<T>> logger)
    {
        Context = context;
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
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.GetMany)} {typeName}")
                .WithName($"Get{groupName}");
            getRouteHandlers.Add(getMany);
        }

        if (CrudOperation.HasFlag(CrudOperation.Post))
        {
            var post = app.MapPost($"/api/{typeName}", AddAsync)
                .Produces<T>(302)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.Post)} {typeName}")
                .WithName($"Post{typeName}");
            postRouteHandlers.Add(post);
        }

        if (CrudOperation.HasFlag(CrudOperation.Put))
        {
            var put = app.MapPut($"/api/{typeName}", UpdateAsync)
                .Produces<T>(302)
                .WithTags(groupName)
                .WithMetadata(type)
                .WithDescription($"{nameof(CrudOperation.Put)} {typeName}")
                .WithName($"Put{typeName}");
            putRouteHandlers.Add(put);
        }

        if (CrudOperation.HasFlag(CrudOperation.Delete))
        {
            var delete = app.MapDelete($"/api/{typeName}", RemoveAsync)
                .Produces(200)
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
    /// <param name="claimsPrincipal"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> AddManyAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] IEnumerable<T> items)
    {
        var list = items.ToList();
        await Context.Set<T>().AddRangeAsync(list);
        await Context.BulkSaveChangesAsync();
        return Ok(list);
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
    /// <param name="claimsPrincipal"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> GetAsync(
        ClaimsPrincipal claimsPrincipal,
        Guid id)
    {
        try
        {
            if (OnBeforeGet is not null)
            {
                await OnBeforeGet.Invoke(claimsPrincipal, id);
            }

            OnBeforeGetAsync?.Invoke(claimsPrincipal, id);

            Logger.LogInformation("Getting {TypeName} for id {Id}", typeof(T).Name, id);
            var item = await Context.Set<T>().FindAsync(id);
            if (item is null)
            {
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
            Logger.LogError(ex, "Error on GetAsync");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Overridable base method for GET Many method
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> GetManyAsync(
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
            var items = Context.Set<T>().AsQueryable();
            var result = await DataSourceLoader.LoadAsync(items, options);

            if (OnAfterGetMany is not null)
            {
                await OnAfterGetMany.Invoke(result);
            }

            OnAfterGetManyAsync?.Invoke(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on GetManyAsync");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Overridable base method for POST method
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> AddAsync(
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

            await Context.Set<T>().AddAsync(item);
            await Context.SaveChangesAsync();

            if (OnAfterPost is not null)
            {
                await OnAfterPost.Invoke(item);
            }

            OnAfterPostAsync?.Invoke(item);

            return CreatedAtRoute($"Get{typeName}", new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on AddAsync");
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Overridable base method for PUT method
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> UpdateAsync(
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

            var result = await Context.Set<T>().FindAsync(item.Id);

            if (result is null)
            {
                return NotFound();
            }

            Context.Entry(result).CurrentValues.SetValues(item);
            await Context.SaveChangesAsync();

            if (OnAfterPut is not null)
            {
                await OnAfterPut.Invoke(item);
            }

            OnAfterPutAsync?.Invoke(item);

            return CreatedAtRoute($"Get{typeName}", new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on UpdateAsync");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Overridable base method for DELETE method
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="claimsPrincipal"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> RemoveAsync(
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
            var result = await Context.Set<T>().FindAsync(id);
            if (result is null)
            {
                return NotFound();
            }

            Context.Set<T>().Remove(result);
            await Context.SaveChangesAsync();

            OnAfterDeleteAsync?.Invoke();
            if (OnAfterDelete is not null)
            {
                await OnAfterDelete.Invoke();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error on RemoveAsync");
            return BadRequest(ex.Message);
        }
    }
}
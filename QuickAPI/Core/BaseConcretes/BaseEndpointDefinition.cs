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
using Swashbuckle.AspNetCore.Annotations;
using static Microsoft.AspNetCore.Http.Results;

namespace QuickAPI.Core.BaseConcretes;

public class BaseEndpointDefinition<T> : EndPointDefinitionBase, IEndpointDefinition
    where T : BaseModel
{
    private readonly BaseContext _context;
    private readonly ILogger<BaseEndpointDefinition<T>> _logger;

    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeGet;

    protected Func<ClaimsPrincipal, DataSourceLoadOptionsBase?, Task>? OnBeforeGetMany;

    protected Func<ClaimsPrincipal, T, Task>? OnBeforePost;

    protected Func<ClaimsPrincipal, T, Task>? OnBeforePut;

    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeDelete;

    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeGetAsync;

    protected Func<ClaimsPrincipal, DataSourceLoadOptionsBase?, Task>? OnBeforeGetManyAsync;

    protected Func<ClaimsPrincipal, T, Task>? OnBeforePostAsync;

    protected Func<ClaimsPrincipal, T, Task>? OnBeforePutAsync;

    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeDeleteAsync;

    protected Func<T, Task>? OnAfterGet;

    protected Func<LoadResult, Task>? OnAfterGetMany;

    protected Func<T, Task>? OnAfterPost;

    protected Func<T, Task>? OnAfterPut;

    protected Func<Task>? OnAfterDelete;

    protected Func<T, Task>? OnAfterGetAsync;

    protected Func<LoadResult, Task>? OnAfterGetManyAsync;

    protected Func<T, Task>? OnAfterPostAsync;

    protected Func<T, Task>? OnAfterPutAsync;

    protected Func<Task>? OnAfterDeleteAsync;

    public BaseEndpointDefinition(
        BaseContext context,
        ILogger<BaseEndpointDefinition<T>> logger)
    {
        _context = context;
        _logger = logger;
    }

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


    protected virtual async Task<IResult> AddManyAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] IEnumerable<T> items)
    {
        var list = items.ToList();
        await _context.Set<T>().AddRangeAsync(list);
        await _context.BulkSaveChangesAsync();
        return Ok(list);
    }

    public override void DefineServices(IServiceCollection services)
    {
    }

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

            _logger.LogInformation("Getting {TypeName} for id {Id}", typeof(T).Name, id);
            var item = await _context.Set<T>().FindAsync(id);
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
            _logger.LogError(ex, "Error on GetAsync");
            return BadRequest(ex.Message);
        }
    }

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

            _logger.LogInformation("Getting {TypeName} items", typeof(T).Name);
            var items = _context.Set<T>().AsQueryable();
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
            _logger.LogError(ex, "Error on GetManyAsync");
            return BadRequest(ex.Message);
        }
    }

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

            _logger.LogInformation("Adding new {TypeName}", typeName);

            await _context.Set<T>().AddAsync(item);
            await _context.SaveChangesAsync();

            if (OnAfterPost is not null)
            {
                await OnAfterPost.Invoke(item);
            }

            OnAfterPostAsync?.Invoke(item);

            return CreatedAtRoute($"Get{typeName}", new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on AddAsync");
            return BadRequest(ex.Message);
        }
    }

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
            _logger.LogInformation("Updating {TypeName}", typeName);

            var result = await _context.Set<T>().FindAsync(item.Id);

            if (result is null)
            {
                return NotFound();
            }

            _context.Entry(result).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();

            if (OnAfterPut is not null)
            {
                await OnAfterPut.Invoke(item);
            }

            OnAfterPutAsync?.Invoke(item);

            return CreatedAtRoute($"Get{typeName}", new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on UpdateAsync");
            return BadRequest(ex.Message);
        }
    }

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
            var result = await _context.Set<T>().FindAsync(id);
            if (result is null)
            {
                return NotFound();
            }

            _context.Set<T>().Remove(result);
            await _context.SaveChangesAsync();

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
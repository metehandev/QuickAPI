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
using MY.QuickAPI.Shared.Dtos;
using static Microsoft.AspNetCore.Http.Results;

namespace MY.QuickAPI.Core.BaseConcretes;

/// <summary>
/// Base endpoint definition class for endpoints that use DTOs for input/output
/// </summary>
/// <typeparam name="TModel">The entity model type</typeparam>
/// <typeparam name="TDto">The DTO type used for input/output</typeparam>
public class BaseDtoEndpointDefinition<TModel, TDto> : EndPointDefinitionBase, IEndpointDefinition
    where TModel : BaseModel, new()
    where TDto : BaseDto, new()
{
    /// <summary>
    /// Injected DbContext
    /// </summary>
    protected readonly BaseContext Context;
    /// <summary>
    /// Injected Logger
    /// </summary>
    protected readonly ILogger<BaseDtoEndpointDefinition<TModel, TDto>> Logger;
    /// <summary>
    /// Injected Mapper
    /// </summary>
    protected readonly IModelDtoMapper<TModel, TDto> Mapper;

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
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePost;
    
    /// <summary>
    /// Assign method to run sync events on before PUT method
    /// </summary>
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePut;
    
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
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePostAsync;
    
    /// <summary>
    /// Assign method to run async events on before PUT method
    /// </summary>
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePutAsync;
    
    /// <summary>
    /// Assign method to run async events on before DELETE method
    /// </summary>
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeDeleteAsync;

    // After event handlers
    
    /// <summary>
    /// Assign method to run sync events on after GET method
    /// </summary>
    protected Func<TDto, Task>? OnAfterGet;
    
    /// <summary>
    /// Assign method to run sync events on after GET Many method
    /// </summary>
    protected Func<LoadResult, Task>? OnAfterGetMany;
    
    /// <summary>
    /// Assign method to run sync events on after POST method
    /// </summary>
    protected Func<TDto, Task>? OnAfterPost;
    
    /// <summary>
    /// Assign method to run sync events on after PUT method
    /// </summary>
    protected Func<TDto, Task>? OnAfterPut;
    
    /// <summary>
    /// Assign method to run sync events on after DELETE method
    /// </summary>
    protected Func<Task>? OnAfterDelete;

    // Async After event handlers
    /// <summary>
    /// Assign method to run async events on after GET method
    /// </summary>
    protected Func<TDto, Task>? OnAfterGetAsync;
    
    /// <summary>
    /// Assign method to run async events on after GET Many method
    /// </summary>
    protected Func<LoadResult, Task>? OnAfterGetManyAsync;
    
    /// <summary>
    /// Assign method to run async events on after POST method
    /// </summary>
    protected Func<TDto, Task>? OnAfterPostAsync;
    
    /// <summary>
    /// Assign method to run async events on after PUT method
    /// </summary>
    protected Func<TDto, Task>? OnAfterPutAsync;
    
    /// <summary>
    /// Assign method to run async events on after DELETE method
    /// </summary>
    protected Func<Task>? OnAfterDeleteAsync;

    /// <summary>
    /// Base constructor for BaseDtoEndpointDefinition
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    public BaseDtoEndpointDefinition(
        BaseContext context,
        ILogger<BaseDtoEndpointDefinition<TModel, TDto>> logger,
        IModelDtoMapper<TModel, TDto> mapper)
    {
        Context = context;
        Logger = logger;
        Mapper = mapper;
    }

    /// <summary>
    /// Define method to set REST Method mappings using minimal APIs 
    /// </summary>
    /// <param name="app"></param>
    public override void Define(WebApplication app)
    {
        var modelType = typeof(TModel);
        var dtoType = typeof(TDto);
        var typeName = modelType.Name;
        var groupName = typeName + "s";

        var getRouteHandlers = new List<RouteHandlerBuilder>();
        var postRouteHandlers = new List<RouteHandlerBuilder>();
        var putRouteHandlers = new List<RouteHandlerBuilder>();
        var deleteRouteHandlers = new List<RouteHandlerBuilder>();

        if (CrudOperation.HasFlag(CrudOperation.Get))
        {
            var get = app.MapGet($"/api/{typeName}", GetAsync)
                .Produces<TDto>()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(dtoType)
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
                .WithMetadata(dtoType)
                .WithDescription($"{nameof(CrudOperation.GetMany)} {typeName}")
                .WithName($"Get{groupName}");
            getRouteHandlers.Add(getMany);
        }

        if (CrudOperation.HasFlag(CrudOperation.Post))
        {
            var post = app.MapPost($"/api/{typeName}", AddAsync)
                .Produces<TDto>(StatusCodes.Status302Found)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(dtoType)
                .WithDescription($"{nameof(CrudOperation.Post)} {typeName}")
                .WithName($"Post{typeName}");
            postRouteHandlers.Add(post);
        }

        if (CrudOperation.HasFlag(CrudOperation.Put))
        {
            var put = app.MapPut($"/api/{typeName}", UpdateAsync)
                .Produces<TDto>(StatusCodes.Status302Found)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(dtoType)
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
                .WithMetadata(dtoType)
                .WithDescription($"{nameof(CrudOperation.Delete)} {typeName}")
                .WithName($"Delete{typeName}");
            deleteRouteHandlers.Add(delete);
        }

        if (CrudOperation.HasFlag(CrudOperation.PostMany))
        {
            var postMany = app.MapPost($"/api/{groupName}", AddManyAsync)
                .Produces<IEnumerable<TDto>>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithTags(groupName)
                .WithMetadata(dtoType)
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
    /// <param name="dtos"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> AddManyAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] IEnumerable<TDto> dtos)
    {
        try
        {
            var entities = Mapper.MapToModels(dtos).ToList();
            
            await Context.Set<TModel>().AddRangeAsync(entities);
            await Context.SaveChangesAsync();
            
            var resultDtos = Mapper.MapToDtos(entities).ToList();
            return Ok(resultDtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on AddManyAsync for {TypeName}: {ErrorMessage}", typeof(TModel).Name, ex.Message);
            return BadRequest($"An error occurred while adding multiple {typeof(TModel).Name} items: {ex.Message}");
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
    /// <param name="claimsPrincipal"></param>
    /// <param name="id"></param>
    /// <param name="includeFields"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> GetAsync(
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

            Logger.LogInformation("Getting {TypeName} for id {Id}", typeof(TModel).Name, id);
            var dbSet = IncludeNavigations(Context.Set<TModel>().AsNoTracking(), includeFields);
            var entity = await dbSet.FirstOrDefaultAsync(m => m.Id == id);
            if (entity is null)
            {
                Logger.LogWarning("No {TypeName} found for id {Id}", typeof(TModel).Name, id);
                return NotFound();
            }

            var dto = Mapper.MapToDto(entity);

            if (OnAfterGet is not null)
            {
                await OnAfterGet.Invoke(dto);
            }

            OnAfterGetAsync?.Invoke(dto);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on GetAsync for {TypeName} id {Id}: {ErrorMessage}", typeof(TModel).Name, id, ex.Message);
            return BadRequest($"An error occurred while retrieving {typeof(TModel).Name} with id {id}: {ex.Message}");
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

            Logger.LogInformation("Getting {TypeName} items", typeof(TModel).Name);
            
            var dbSet = IncludeNavigations(Context.Set<TModel>().AsNoTracking(), options.IncludeFields);
            var result = await DataSourceLoader.LoadAsync(dbSet, options);

            result.data = Mapper.MapToDtos(result.data.Cast<TModel>()).ToList();
            
            if (OnAfterGetMany is not null)
            {
                await OnAfterGetMany.Invoke(result);
            }

            OnAfterGetManyAsync?.Invoke(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on GetManyAsync for {TypeName}: {ErrorMessage}", typeof(TModel).Name, ex.Message);
            return BadRequest($"An error occurred while retrieving list of {typeof(TModel).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Overridable base method for POST method
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> AddAsync(
        ClaimsPrincipal claimsPrincipal,
        TDto dto)
    {
        try
        {
            var type = typeof(TModel);
            var typeName = type.Name;

            if (OnBeforePost is not null)
            {
                await OnBeforePost.Invoke(claimsPrincipal, dto);
            }

            OnBeforePostAsync?.Invoke(claimsPrincipal, dto);

            Logger.LogInformation("Adding new {TypeName}", typeName);

            var entity = Mapper.MapToModel(dto);
            await Context.Set<TModel>().AddAsync(entity);
            await Context.SaveChangesAsync();
            
            // Map back to DTO to get any generated IDs or defaults
            var resultDto = Mapper.MapToDto(entity);

            if (OnAfterPost is not null)
            {
                await OnAfterPost.Invoke(resultDto);
            }

            OnAfterPostAsync?.Invoke(resultDto);

            return CreatedAtRoute($"Get{typeName}", new { id = resultDto.Id }, resultDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on AddAsync for {TypeName}: {ErrorMessage}", typeof(TModel).Name, ex.Message);
            return BadRequest($"An error occurred while adding {typeof(TModel).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Overridable base method for PUT method
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    protected virtual async Task<IResult> UpdateAsync(
        ClaimsPrincipal claimsPrincipal,
        TDto dto)
    {
        try
        {
            if (OnBeforePut is not null)
            {
                await OnBeforePut.Invoke(claimsPrincipal, dto);
            }

            OnBeforePutAsync?.Invoke(claimsPrincipal, dto);

            var type = typeof(TModel);
            var typeName = type.Name;
            Logger.LogInformation("Updating {TypeName}", typeName);

            var existingEntity = await Context.Set<TModel>().FindAsync(dto.Id);
            if (existingEntity is null)
            {
                Logger.LogWarning("No {TypeName} found to update for id {Id}", typeof(TModel).Name, dto.Id);
                return NotFound();
            }

            // Map DTO to existing entity
            var updatedEntity = Mapper.MapToModel(dto, existingEntity);
            
            // EF Core will track the changes
            await Context.SaveChangesAsync();
            
            // Map back to DTO for response
            var resultDto = Mapper.MapToDto(updatedEntity);

            if (OnAfterPut is not null)
            {
                await OnAfterPut.Invoke(resultDto);
            }

            OnAfterPutAsync?.Invoke(resultDto);

            return CreatedAtRoute($"Get{typeName}", new { id = resultDto.Id }, resultDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on UpdateAsync for {TypeName} id {Id}: {ErrorMessage}", typeof(TModel).Name, dto.Id, ex.Message);
            return BadRequest($"An error occurred while updating {typeof(TModel).Name} with id {dto.Id}: {ex.Message}");
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
        ILogger<BaseDtoEndpointDefinition<TModel, TDto>> logger,
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

            logger.LogInformation("Deleting {TypeName} for id {Id}", typeof(TModel).Name, id);
            var entity = await Context.Set<TModel>().FindAsync(id);
            if (entity is null)
            {
                logger.LogWarning("No {TypeName} found to delete for id {Id}", typeof(TModel).Name, id);
                return NotFound();
            }

            Context.Set<TModel>().Remove(entity);
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
            logger.LogError(ex, "Error on RemoveAsync for {TypeName} id {Id}: {ErrorMessage}", typeof(TModel).Name, id, ex.Message);
            return BadRequest($"An error occurred while deleting {typeof(TModel).Name} with id {id}: {ex.Message}");
        }
    }
    
    private IQueryable<TModel> IncludeNavigations(IQueryable<TModel> dbSet, string[]? includeFields)
    {
        if (includeFields?.Length > 0)
        {
            foreach (var includeField in includeFields)
            {
                dbSet = dbSet.Include(includeField);
            }

            return dbSet;
        }

        if (IncludeFields.Length > 0)
        {
            foreach (var includeField in IncludeFields)
            {
                dbSet = dbSet.Include(includeField);
            }

            return dbSet;
        }

        return dbSet;
    }
}
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
using QuickAPI.Shared.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using static Microsoft.AspNetCore.Http.Results;

namespace QuickAPI.Core.BaseConcretes;

/// <summary>
/// Base endpoint definition class for endpoints that use DTOs for input/output
/// </summary>
/// <typeparam name="TModel">The entity model type</typeparam>
/// <typeparam name="TDto">The DTO type used for input/output</typeparam>
public class BaseDtoEndpointDefinition<TModel, TDto> : EndPointDefinitionBase, IEndpointDefinition
    where TModel : BaseModel, new()
    where TDto : BaseDto, new()
{
    protected readonly BaseContext Context;
    protected readonly ILogger<BaseDtoEndpointDefinition<TModel, TDto>> Logger;
    protected readonly IModelDtoMapper<TModel, TDto> Mapper;

    // Before event handlers
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeGet;
    protected Func<ClaimsPrincipal, DataSourceLoadOptionsBase?, Task>? OnBeforeGetMany;
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePost;
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePut;
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeDelete;

    // Async Before event handlers
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeGetAsync;
    protected Func<ClaimsPrincipal, DataSourceLoadOptionsBase?, Task>? OnBeforeGetManyAsync;
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePostAsync;
    protected Func<ClaimsPrincipal, TDto, Task>? OnBeforePutAsync;
    protected Func<ClaimsPrincipal, Guid, Task>? OnBeforeDeleteAsync;

    // After event handlers
    protected Func<TDto, Task>? OnAfterGet;
    protected Func<LoadResult, Task>? OnAfterGetMany;
    protected Func<TDto, Task>? OnAfterPost;
    protected Func<TDto, Task>? OnAfterPut;
    protected Func<Task>? OnAfterDelete;

    // Async After event handlers
    protected Func<TDto, Task>? OnAfterGetAsync;
    protected Func<LoadResult, Task>? OnAfterGetManyAsync;
    protected Func<TDto, Task>? OnAfterPostAsync;
    protected Func<TDto, Task>? OnAfterPutAsync;
    protected Func<Task>? OnAfterDeleteAsync;

    public BaseDtoEndpointDefinition(
        BaseContext context,
        ILogger<BaseDtoEndpointDefinition<TModel, TDto>> logger,
        IModelDtoMapper<TModel, TDto> mapper)
    {
        Context = context;
        Logger = logger;
        Mapper = mapper;
    }

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
                .WithTags(groupName)
                .WithMetadata(dtoType)
                .WithDescription($"{nameof(CrudOperation.GetMany)} {typeName}")
                .WithName($"Get{groupName}");
            getRouteHandlers.Add(getMany);
        }

        if (CrudOperation.HasFlag(CrudOperation.Post))
        {
            var post = app.MapPost($"/api/{typeName}", AddAsync)
                .Produces<TDto>(302)
                .WithTags(groupName)
                .WithMetadata(dtoType)
                .WithDescription($"{nameof(CrudOperation.Post)} {typeName}")
                .WithName($"Post{typeName}");
            postRouteHandlers.Add(post);
        }

        if (CrudOperation.HasFlag(CrudOperation.Put))
        {
            var put = app.MapPut($"/api/{typeName}", UpdateAsync)
                .Produces<TDto>(302)
                .WithTags(groupName)
                .WithMetadata(dtoType)
                .WithDescription($"{nameof(CrudOperation.Put)} {typeName}")
                .WithName($"Put{typeName}");
            putRouteHandlers.Add(put);
        }

        if (CrudOperation.HasFlag(CrudOperation.Delete))
        {
            var delete = app.MapDelete($"/api/{typeName}", RemoveAsync)
                .Produces(200)
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

    protected virtual async Task<IResult> AddManyAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromBody] IEnumerable<TDto> dtos)
    {
        try
        {
            var entities = Mapper.MapToModels(dtos).ToList();
            
            await Context.Set<TModel>().AddRangeAsync(entities);
            await Context.BulkSaveChangesAsync();
            
            var resultDtos = Mapper.MapToDtos(entities).ToList();
            return Ok(resultDtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error on AddManyAsync");
            return BadRequest(ex.Message);
        }
    }

    public override void DefineServices(IServiceCollection services)
    {
        // Register mapper service if needed
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

            Logger.LogInformation("Getting {TypeName} for id {Id}", typeof(TModel).Name, id);
            var entity = await Context.Set<TModel>().FindAsync(id);
            if (entity is null)
            {
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
            Logger.LogError(ex, "Error on GetAsync");
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

            Logger.LogInformation("Getting {TypeName} items", typeof(TModel).Name);
            var items = Context.Set<TModel>().AsQueryable();
            var result = await DataSourceLoader.LoadAsync(items, options);

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
            Logger.LogError(ex, "Error on GetManyAsync");
            return BadRequest(ex.Message);
        }
    }

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
            Logger.LogError(ex, "Error on AddAsync");
            return BadRequest(ex.Message);
        }
    }

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
            Logger.LogError(ex, "Error on UpdateAsync");
            return BadRequest(ex.Message);
        }
    }

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
            logger.LogError(ex, "Error on RemoveAsync");
            return BadRequest(ex.Message);
        }
    }
}
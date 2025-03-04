using Microsoft.Extensions.Logging;
using QuickAPI.Database.DataModels;
using QuickAPI.Shared.Dtos;

namespace QuickAPI.Core.BaseConcretes;

/// <summary>
/// A simple implementation of IModelDtoMapper that requires manual property mapping in derived classes
/// </summary>
/// <typeparam name="TModel">The entity model type</typeparam>
/// <typeparam name="TDto">The DTO type</typeparam>
public abstract class SimpleModelDtoMapper<TModel, TDto>(ILogger<SimpleModelDtoMapper<TModel, TDto>> logger)
    : IModelDtoMapper<TModel, TDto>
    where TModel : BaseModel, new()
    where TDto : BaseDto, new()
{
    /// <summary>
    /// Base logger
    /// </summary>
    protected readonly ILogger<SimpleModelDtoMapper<TModel, TDto>> Logger = logger;

    /// <summary>
    /// Override this method to set mappings between TModel => TDto
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public abstract TDto MapToDto(TModel model);

    /// <summary>
    /// Override this method to set mappings between TDto => TModel
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public abstract TModel MapToModel(TDto dto, TModel? model = null);

    /// <summary>
    /// Base method to map models to Dtos using the MapToDto single method
    /// </summary>
    /// <param name="models"></param>
    /// <returns></returns>
    public virtual IEnumerable<TDto> MapToDtos(IEnumerable<TModel> models)
    {
        return models.Select(MapToDto);
    }

    /// <summary>
    /// Base method to map Dtos to models using the MapToModel method 
    /// </summary>
    /// <param name="dtos"></param>
    /// <returns></returns>
    public virtual IEnumerable<TModel> MapToModels(IEnumerable<TDto> dtos)
    {
        return dtos.Select(dto => MapToModel(dto));
    }
}
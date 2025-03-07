using MY.QuickAPI.Database.DataModels;
using MY.QuickAPI.Shared.Dtos;

namespace MY.QuickAPI.Core;

/// <summary>
/// Interface for mapping between entity models and DTOs
/// </summary>
/// <typeparam name="TModel">The entity model type</typeparam>
/// <typeparam name="TDto">The DTO type</typeparam>
public interface IModelDtoMapper<TModel, TDto> : IModelDtoMapper
    where TModel : BaseModel
    where TDto : BaseDto
{
    /// <summary>
    /// Maps an entity model to a DTO
    /// </summary>
    /// <param name="model">The entity model to map</param>
    /// <returns>A new DTO instance with values from the model</returns>
    TDto? MapToDto(TModel? model);
    
    /// <summary>
    /// Maps a DTO to an entity model
    /// </summary>
    /// <param name="dto">The DTO to map</param>
    /// <param name="model">Optional existing model to update. If null, a new model will be created.</param>
    /// <returns>The updated or new entity model</returns>
    TModel? MapToModel(TDto? dto, TModel? model = null);
    
    /// <summary>
    /// Maps a collection of entity models to DTOs
    /// </summary>
    /// <param name="models">The collection of entity models</param>
    /// <returns>A collection of mapped DTOs</returns>
    IEnumerable<TDto> MapToDtos(IEnumerable<TModel> models);
    
    /// <summary>
    /// Maps a collection of DTOs to entity models
    /// </summary>
    /// <param name="dtos">The collection of DTOs</param>
    /// <returns>A collection of mapped entity models</returns>
    IEnumerable<TModel> MapToModels(IEnumerable<TDto> dtos);
}
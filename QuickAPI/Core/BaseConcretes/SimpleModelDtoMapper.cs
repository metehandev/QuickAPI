using Microsoft.Extensions.Logging;
using QuickAPI.Database.DataModels;
using QuickAPI.Shared.Dtos;

namespace QuickAPI.Core.BaseConcretes;

/// <summary>
/// A simple implementation of IModelDtoMapper that requires manual property mapping in derived classes
/// </summary>
/// <typeparam name="TModel">The entity model type</typeparam>
/// <typeparam name="TDto">The DTO type</typeparam>
public abstract class SimpleModelDtoMapper<TModel, TDto> : IModelDtoMapper<TModel, TDto>
    where TModel : BaseModel, new()
    where TDto : BaseDto, new()
{
    protected readonly ILogger<SimpleModelDtoMapper<TModel, TDto>> Logger;

    protected SimpleModelDtoMapper(ILogger<SimpleModelDtoMapper<TModel, TDto>> logger)
    {
        Logger = logger;
    }

    public abstract TDto MapToDto(TModel model);

    public abstract TModel MapToModel(TDto dto, TModel? model = null);

    public virtual IEnumerable<TDto> MapToDtos(IEnumerable<TModel> models)
    {
        return models.Select(MapToDto);
    }

    public virtual IEnumerable<TModel> MapToModels(IEnumerable<TDto> dtos)
    {
        return dtos.Select(dto => MapToModel(dto));
    }
}
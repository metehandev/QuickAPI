using QuickAPI.Core;
using QuickAPI.Example.DataModels;
using QuickAPI.Example.Dtos;

namespace QuickAPI.Example.Mappers;

/// <summary>
/// Mapper for converting between Category entity and CategoryDto
/// </summary>
public class CategoryMapper : IModelDtoMapper<Category, CategoryDto>
{
    public CategoryDto MapToDto(Category model)
    {
        var dto = new CategoryDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description ?? string.Empty
        };

        // You could calculate product count here if needed
        // dto.ProductCount = _context.Set<Product>().Count(p => p.CategoryId == model.Id);

        return dto;
    }

    public Category MapToModel(CategoryDto dto, Category? model = null)
    {
        model ??= new Category();

        model.Id = dto.Id;
        model.Name = dto.Name; // Using Name field for Name
        model.Description = dto.Description; // Using Description field for Description

        return model;
    }

    public IEnumerable<CategoryDto> MapToDtos(IEnumerable<Category> models)
    {
        return models.Select(MapToDto);
    }

    public IEnumerable<Category> MapToModels(IEnumerable<CategoryDto> dtos)
    {
        return dtos.Select(dto => MapToModel(dto));
    }
}
using MY.QuickAPI.Core;
using MY.QuickAPI.Example.DataModels;
using MY.QuickAPI.Example.Dtos;

namespace MY.QuickAPI.Example.Mappers;

/// <summary>
/// Mapper for converting between Category entity and CategoryDto
/// </summary>
public class CategoryMapper : SimpleModelDtoMapper<Category, CategoryDto>
{

    public CategoryMapper(ILogger<CategoryMapper> logger) : base(logger)
    {
    }
    
    public override CategoryDto? MapToDto(Category? model)
    {
        if (model is null)
        {
            return null;
        }
        
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

    public override Category? MapToModel(CategoryDto? dto, Category? model = null)
    {
        if (dto is null)
        {
            return null;
        }
        
        model ??= new Category();

        model.Id = dto.Id;
        model.Name = dto.Name; // Using Name field for Name
        model.Description = dto.Description; // Using Description field for Description

        return model;
    }
}
using MY.QuickAPI.Core;
using MY.QuickAPI.Example.DataModels;
using MY.QuickAPI.Example.Dtos;

namespace MY.QuickAPI.Example.Mappers;

/// <summary>
/// Mapper for converting between Product entity and ProductDto
/// </summary>
public class ProductMapper : SimpleModelDtoMapper<Product, ProductDto>
{
    private readonly IModelDtoMapper<Category, CategoryDto> _categoryMapper;

    public ProductMapper(ILogger<ProductMapper> logger,
        IModelDtoMapper<Category, CategoryDto> categoryMapper) : base(logger)
    {
        _categoryMapper = categoryMapper;
    }

    public override ProductDto? MapToDto(Product? model)
    {
        if (model is null)
        {
            return null;
        }
        
        var dto = new ProductDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            CategoryId = model.CategoryId,
            CategoryName = model.Category?.Name,
            Category = _categoryMapper.MapToDto(model.Category)
        };

        return dto;
    }

    public override Product? MapToModel(ProductDto? dto, Product? model = null)
    {
        if (dto is null)
        {
            return null;
        }
        
        model ??= new Product();

        model.Id = dto.Id;
        model.Name = dto.Name;               // Using Name field for Name
        model.Description = dto.Description; // Using Description field for Description
        model.CategoryId = dto.CategoryId;

        return model;
    }
}
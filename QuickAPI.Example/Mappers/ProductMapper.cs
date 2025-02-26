using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickAPI.Core;
using QuickAPI.Database.Data;
using QuickAPI.Example.DataModels;
using QuickAPI.Example.Dtos;

namespace QuickAPI.Example.Mappers;

/// <summary>
/// Mapper for converting between Product entity and ProductDto
/// </summary>
public class ProductMapper : IModelDtoMapper<Product, ProductDto>
{
    private readonly ILogger<ProductMapper> _logger;
    private readonly BaseContext _context;

    public ProductMapper(ILogger<ProductMapper> logger, BaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public ProductDto MapToDto(Product model)
    {
        var dto = new ProductDto
        {
            Id = model.Id,
            Code = model.Name,
            Info = model.Description ?? string.Empty,
            CategoryId = model.CategoryId,
            CategoryName = model.Category?.Name
        };

        return dto;
    }

    public Product MapToModel(ProductDto dto, Product? model = null)
    {
        model ??= new Product();

        model.Id = dto.Id;
        model.Name = dto.Code; // Using Code field for Name
        model.Description = dto.Info; // Using Info field for Description
        model.CategoryId = dto.CategoryId;

        return model;
    }

    public IEnumerable<ProductDto> MapToDtos(IEnumerable<Product> models)
    {
        return models.Select(MapToDto);
    }

    public IEnumerable<Product> MapToModels(IEnumerable<ProductDto> dtos)
    {
        return dtos.Select(dto => MapToModel(dto));
    }
}
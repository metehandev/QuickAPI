using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickAPI.Core;
using QuickAPI.Core.BaseConcretes;
using QuickAPI.Database.Data;
using QuickAPI.Example.DataModels;
using QuickAPI.Example.Dtos;

namespace QuickAPI.Example.Mappers;

/// <summary>
/// Mapper for converting between Category entity and CategoryDto
/// </summary>
public class CategoryMapper : IModelDtoMapper<Category, CategoryDto>
{
    private readonly ILogger<CategoryMapper> _logger;
    private readonly BaseContext _context;

    public CategoryMapper(ILogger<CategoryMapper> logger, BaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public CategoryDto MapToDto(Category model)
    {
        var dto = new CategoryDto
        {
            Id = model.Id,
            Code = model.Name,
            Info = model.Description ?? string.Empty
        };

        // You could calculate product count here if needed
        // dto.ProductCount = _context.Set<Product>().Count(p => p.CategoryId == model.Id);

        return dto;
    }

    public Category MapToModel(CategoryDto dto, Category? model = null)
    {
        model ??= new Category();

        model.Id = dto.Id;
        model.Name = dto.Code; // Using Code field for Name
        model.Description = dto.Info; // Using Info field for Description

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
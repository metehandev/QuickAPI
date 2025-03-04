// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using MY.QuickAPI.Core;
// using MY.MY.QuickAPI.Example.DataModels;
// using MY.MY.QuickAPI.Example.Dtos;
// using MY.MY.QuickAPI.Example.Mappers;
//
// namespace MY.MY.QuickAPI.Example.Definitions;
//
// /// <summary>
// /// Definition class to register all mappers in the DI container
// /// </summary>
// public class MappersDefinition : IDefinition
// {
//     public void Define(WebApplication app)
//     {
//         // Nothing to define in the app
//     }
//
//     public void DefineServices(IServiceCollection services)
//     {
//         // Example 1: Register the ProductMapper for the Product-ProductDto mapping
//         services.AddTransient<IModelDtoMapper<Product, ProductDto>, ProductMapper>();
//         
//         // Example 1: Register the CategoryMapper for the Category-CategoryDto mapping
//         services.AddTransient<IModelDtoMapper<Category, CategoryDto>, CategoryMapper>();
//     }
// }
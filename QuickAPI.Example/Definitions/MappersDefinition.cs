// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using QuickAPI.Core;
// using QuickAPI.Example.DataModels;
// using QuickAPI.Example.Dtos;
// using QuickAPI.Example.Mappers;
//
// namespace QuickAPI.Example.Definitions;
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
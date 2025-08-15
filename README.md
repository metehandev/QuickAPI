# MY.QuickAPI

MY.QuickAPI is a .NET library that provides a set of powerful attributes for configuring database tables, columns, and constraints in a code-first approach. It simplifies the process of defining database schemas through C# attributes and automatically generates CRUD API endpoints.

Important: DI + Endpoint Lifecycle
- Endpoints are instantiated twice: once during `AddQuickApi(...)` only to let them register services via `DefineServices(IServiceCollection)`, and once during `UseQuickApi(app)` to map routes using the final `app.Services` provider.
- Do not resolve scoped services (e.g., EF `DbContext`, `IHubContext`) in endpoint constructors. Instead, use method-parameter DI in your handlers (see examples below). Injecting `IServiceProvider` in the ctor is allowed, but avoid resolving scoped dependencies there.

## Database Attributes

### Table Configuration

#### `SqlConstraintIndexAttribute`
Defines indexes on database tables with support for both clustered and non-clustered indexes.

```csharp
[SqlConstraintIndex(nameof(Email), IsUnique = true)]
[SqlConstraintIndex(nameof(LastName), nameof(FirstName), IsClustered = true)]
public class User 
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

#### `SqlPrimaryKeyAttribute`
Customizes the primary key configuration when you need a different setup than the default.

```csharp
[SqlPrimaryKey(nameof(TenantId), nameof(UserId), Name = "PK_TenantUser")]
public class TenantUser
{
    public int TenantId { get; set; }
    public int UserId { get; set; }
}
```

### Column Configuration

#### `SqlComputedAttribute`
Defines computed columns with optional persistence.

```csharp
public class Employee
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [SqlComputed("[FirstName] + ' ' + [LastName]", Stored = true)]
    public string FullName { get; set; }
}
```

#### `SqlDefaultValueAttribute`
Sets default values for columns using SQL expressions or literal values.

```csharp
public class Article
{
    public string Title { get; set; }
    
    [SqlDefaultValue("GETUTCDATE()")]
    public DateTime CreatedAt { get; set; }
    
    [SqlDefaultValue("0")]
    public int ViewCount { get; set; }
}
```

## Endpoint Generation

### Automatic CRUD Endpoints

QuickAPI can automatically generate CRUD endpoints for your models by adding the `EndpointDefinitionAttribute`.

```csharp
[EndpointDefinition]
public class Product : BaseModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}
```

This will generate the following endpoints:
- `GET /api/Product` - Get a product by ID
- `GET /api/Products` - Get multiple products with filtering and sorting
- `POST /api/Product` - Create a new product
- `PUT /api/Product` - Update an existing product
- `DELETE /api/Product` - Delete a product
- `POST /api/Products` - Create multiple products at once

### Using DTOs for API Endpoints

You can use DTOs for input/output models in your API endpoints in two different ways:

#### Approach 1: Using the EndpointDefinition Attribute with DtoType

The simplest approach is to specify the DTO type directly on your model:

```csharp
// Define your DTO
public record ProductDto : BaseDto
{
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

// Create mapper implementation
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
        return new ProductDto
        {
            Id = model.Id,
            Code = model.Name,
            Info = model.Description ?? string.Empty,
            CategoryId = model.CategoryId,
            CategoryName = model.Category?.Name
        };
    }

    public Product MapToModel(ProductDto dto, Product? model = null)
    {
        model ??= new Product();
        model.Id = dto.Id;
        model.Name = dto.Code;
        model.Description = dto.Info;
        model.CategoryId = dto.CategoryId;
        return model;
    }

    // Implement collection mapping methods...
}

// Register the DTO with your model using the attribute
[EndpointDefinition(
    CrudOperation = CrudOperation.All, 
    DtoType = typeof(ProductDto) // Specify the DTO type here
)]
public class Product : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}

// Register the mapper in a Definition class
public class MappersDefinition : IDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddTransient<IModelDtoMapper<Product, ProductDto>, ProductMapper>();
    }
    
    // other methods...
}
```

#### Approach 2: Creating a Custom Endpoint Definition

For more control, you can create a custom endpoint definition:

```csharp
// Define your model and disable automatic endpoint creation
[EndpointDefinition(AutomaticEndpointCreation = false)]
public class Category : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
}

// Define your DTO
public record CategoryDto : BaseDto
{
    public int? ProductCount { get; set; }
}

// Create your mapper
public class CategoryMapper : IModelDtoMapper<Category, CategoryDto>
{
    public CategoryDto MapToDto(Category model)
    {
        return new CategoryDto
        {
            Id = model.Id,
            Code = model.Name,
            Info = model.Description ?? string.Empty
        };
    }
    
    // Other mapper methods...
}

// Create a custom endpoint definition
public class CategoryEndpoint : BaseDtoEndpointDefinition<Category, CategoryDto>
{
    public CategoryEndpoint(
        ILogger<CategoryEndpoint> logger,
        IModelDtoMapper<Category, CategoryDto> mapper)
        : base(logger, mapper)
    {
        // Configure CRUD operations
        CrudOperation = CrudOperation.All;

        // Configure authorization
        RequireAuthorization = true;
        CommonRole = nameof(UserRole.Admin);

        // Optional hooks
        OnBeforeGetMany = async (principal, options) =>
        {
            Logger.LogInformation("Custom before-hook executed");
            await Task.CompletedTask;
        };
    }

    public override void DefineServices(IServiceCollection services)
    {
        services.AddTransient<IModelDtoMapper<Category, CategoryDto>, CategoryMapper>();
    }

    // Use method-parameter DI for scoped services
    protected override async Task<IResult> GetManyAsync(
        BaseContext context,
        ClaimsPrincipal claimsPrincipal,
        BindableDataSourceLoadOptions options)
    {
        Logger.LogInformation("Custom GetManyAsync for CategoryEndpoint");
        // You can use 'context' safely here (scoped per request)
        return await base.GetManyAsync(context, claimsPrincipal, options);
    }
}

// Example: Auth endpoint with method-parameter DI for DbContext
public class AuthenticationEndpoint : EndPointDefinitionBase, IEndpointDefinition
{
    public AuthenticationEndpoint()
    {
        RequireAuthorization = false;
        CommonRole = nameof(UserRole.SuperAdmin);
    }

    public override void Define(WebApplication app)
    {
        const string name = "Authentication";
        app.MapPost($"/api/{name}", LoginAsync)
           .Produces<AuthenticationDto>()
           .WithTags(name)
           .AllowAnonymous();
    }

    protected virtual async Task<IResult> LoginAsync(
        BaseContext context,
        ILogger<AuthenticationEndpoint> logger,
        ITokenService tokenService,
        IConfiguration configuration,
        LoginDto userModel)
    {
        // Validate + read using 'context' (scoped)
        // ...
        return Results.Ok(new AuthenticationDto("user", "token"));
    }

    public override void DefineServices(IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }
}
```

## Usage Examples

### Creating a Table with Multiple Indexes

```csharp
[SqlConstraintIndex(nameof(Email), IsUnique = true, Name = "UQ_User_Email")]
[SqlConstraintIndex(nameof(LastName), nameof(FirstName), IsClustered = true, Name = "IX_User_Name")]
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    [SqlDefaultValue("GETUTCDATE()")]
    public DateTime CreatedAt { get; set; }
    
    [SqlComputed("[FirstName] + ' ' + [LastName]")]
    public string FullName { get; set; }
}
```

### Customizing Authorization for Endpoints

```csharp
[EndpointDefinition(
    CommonRole = "Manager",
    GetRole = "User",
    PostRole = "Admin",
    AllowAnonymousGet = true
)]
public class Customer : BaseModel
{
    public string Name { get; set; }
    public string Email { get; set; }
}
```

## Features

- **Automatic API Generation**: Generate CRUD endpoints with a single attribute
- **DTO Support**: Use Data Transfer Objects for API input/output
- **Flexible Index Configuration**: Create unique, clustered, and non-clustered indexes
- **Computed Columns**: Define computed columns with optional persistence
- **Default Values**: Specify SQL default values for columns
- **Custom Primary Keys**: Configure composite primary keys with custom naming
- **Role-Based Authorization**: Configure endpoint access by role

## Best Practices

1. **DTO Mapping**:
   - Create dedicated DTO classes for external API communication
   - Implement custom mappers for precise control over property mapping
   - Register mappers in your service configuration

2. **Index Naming**:
   - Use meaningful names for indexes
   - Follow a consistent naming convention (e.g., IX_TableName_Columns for non-unique indexes)
   - Use UQ prefix for unique indexes
   - Always use `nameof` operator when referencing properties to maintain type safety

3. **Authorization**:
   - Use role-based authorization to secure endpoints
   - Configure different roles for different HTTP methods as needed
   - Consider which endpoints can be anonymous and which require authentication

4. **Dependency Injection in Endpoints**:
   - Constructors: inject only stable services (e.g., `ILogger<T>`, mappers, options). Avoid resolving scoped services here.
   - Handlers: prefer method-parameter DI for scoped/volatile services (e.g., `BaseContext`, `IHubContext`, `IHttpContextAccessor`).
   - Advanced: if you inject `IServiceProvider` in ctors, do not resolve scoped services there — resolve them within request handlers (or create a scope).

## Setup

```csharp
// Program.cs
builder.Services.AddQuickApi(typeof(Program), typeof(YourMarkerType));
// Optional: builder.Services.AddSignalR(); etc.

var app = builder.Build();
app.UseQuickApi();
// Optional: app.MapHub<YourHub>("/hubs/your");
app.Run();
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 

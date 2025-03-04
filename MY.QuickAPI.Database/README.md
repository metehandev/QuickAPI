# MY.QuickAPI.Database

## Overview

`MY.QuickAPI.Database` is a foundational library for database components in QuickAPI. It provides a set of base models, a core `DbContext` implementation, an interface for tenant-based multi-tenancy support, and custom data annotations for EF Core models.

This package is designed to be **used in backend projects** that require database operations with **Entity Framework Core**.

## Features

- **Base Models**: Provides a `BaseModel` class with common fields such as `Id`, `CreatedAt`, `ModifiedAt`, and `IsDeleted`.
- **Custom EF Core Extensions**: Includes helpers for applying custom attributes and constraints on entity properties.
- **Tenant Support**: Implements `ITenantModel` to enforce tenant-based filtering.
- **Base DbContext**: A `BaseContext` class that automatically applies query filters, tracks modifications, and handles tenant-aware queries.
- **Custom Data Annotations**: Attributes for defining SQL default values, computed columns, primary keys, and constraint indexes.

## Installation

To install this package via NuGet:

```sh
dotnet add package MY.QuickAPI.Database
```

This package **requires EF Core**. Ensure you have installed `Microsoft.EntityFrameworkCore` in your project.

## Usage

### **BaseModel**
All entity models should inherit from `BaseModel` to get common fields and automatic tracking.

```csharp
using QuickAPI.Database.DataModels;

public class Product : BaseModel
{
    public decimal Price { get; set; }
}
```

### **Tenant Support**
To support multi-tenancy, a model should implement `ITenantModel`:

```csharp
using QuickAPI.Database.DataModels;

public class Order : BaseModel, ITenantModel
{
    public Guid TenantId { get; set; }
    public decimal TotalAmount { get; set; }
}
```

### **BaseContext**
The `BaseContext` class automatically applies:
- **Tenant-based filtering** using `_tenantProvider`
- **Timestamps for created and modified fields**
- **Cascade delete restrictions** for safer operations

#### Example Usage:

```csharp
using Microsoft.EntityFrameworkCore;
using QuickAPI.Database.Data;

public class AppDbContext : BaseContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider)
        : base(options, tenantProvider)
    {
    }

    public DbSet<Product> Products { get; set; }
}
```

### **Applying Custom Data Annotations**
Use built-in attributes for defining SQL constraints directly in your models.

#### **Default Values**
```csharp
[SqlDefaultValue("getdate()")]
public DateTimeOffset CreatedAt { get; set; }
```

#### **Computed Columns**
```csharp
[SqlComputed("Price * Quantity", stored: true)]
public decimal TotalCost { get; set; }
```

#### **Primary Key Naming**
```csharp
[SqlPrimaryKey("PK_CustomKey", "Id")]
public class CustomEntity
{
    public int Id { get; set; }
}
```

## Notes

- This package is **only for backend projects**.
- It is meant to be **referenced in QuickAPI** but can be used in any .NET project using EF Core.
- Supports **SQL Server**, **PostgreSQL**, and other EF Core-supported databases.

## License

This project is licensed under the **Apache 2.0 License**.

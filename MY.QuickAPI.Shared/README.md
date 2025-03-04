# MY.uickAPI.Shared

## Overview

`MY.QuickAPI.Shared` is a shared library package designed to be used across both backend and frontend applications. It provides a common set of **Data Transfer Object (DTO)** models to ensure consistency between different parts of the application.

This package simplifies data handling by allowing both backend and frontend to use the same DTO models, reducing duplication and minimizing inconsistencies.

## Features

- **Common DTOs**: Defines shared data models that can be used in both backend and frontend.
- **JSON Serialization**: Uses `System.Text.Json` for efficient serialization and deserialization.
- **Lightweight and Reusable**: Can be referenced in multiple projects without unnecessary dependencies.

## Installation

### Backend (C#)

If you're using the `MY.QuickAPI` package in your backend project, you **don’t need to reference this package separately**, as it is already included.

However, if you want to add it manually, install it via NuGet:

```sh
dotnet add package MY.QuickAPI.Shared
```

### Frontend (Blazor or .NET-based)

If your frontend is built with **Blazor** or any other .NET-based technology, you can reference this package as follows:

```sh
dotnet add package QuickAPI.Shared
```

If your frontend is **not using C#**, this package can be ignored.

## Usage

### BaseDto

The `BaseDto` class serves as the base record for DTO objects. It includes:

- **Id** (`Guid`) - Unique identifier.
- **Name** (`string`) - Entity name.
- **Description** (`string`) - Additional details.
- **ToString() Override** - Returns the JSON representation of the DTO.

```csharp
using QuickAPI.Shared.Dtos;

public record ProductDto : BaseDto
{
    public decimal Price { get; init; }
}
```

### JSON Serialization Example

```csharp
var product = new ProductDto
{
    Id = Guid.NewGuid(),
    Name = "Laptop",
    Description = "High-performance laptop",
    Price = 1500.99M
};

string json = product.ToString();
Console.WriteLine(json);
```

### Example Output

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 1500.99
}
```

## Notes

- This package **should not be included** in the backend project if `QuickAPI` is already referenced.
- This package **can be ignored** if the frontend is **not written in C#**.

## License

This project is licensed under the **Apache 2.0 License**.

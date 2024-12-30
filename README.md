# QuickAPI

QuickAPI is a .NET library that provides a set of powerful attributes for configuring database tables, columns, and constraints in a code-first approach. It simplifies the process of defining database schemas through C# attributes.

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

### Composite Primary Key with Custom Name

```csharp
[SqlPrimaryKey(nameof(OrderId), nameof(ProductId), Name = "PK_OrderProduct")]
public class OrderProduct
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    
    [SqlDefaultValue("1")]
    public int Quantity { get; set; }
}
```

## Features

- **Flexible Index Configuration**: Create unique, clustered, and non-clustered indexes
- **Computed Columns**: Define computed columns with optional persistence
- **Default Values**: Specify SQL default values for columns
- **Custom Primary Keys**: Configure composite primary keys with custom naming
- **Code Generation Support**: Handle nullable types in code generation

## Best Practices

1. **Index Naming**:
   - Use meaningful names for indexes
   - Follow a consistent naming convention (e.g., IX_TableName_Columns for non-unique indexes)
   - Use UQ prefix for unique indexes
   - Always use `nameof` operator when referencing properties to maintain type safety

2. **Computed Columns**:
   - Consider using `Stored = true` for frequently accessed computed columns
   - Use `Stored = false` for simple computations or infrequently accessed columns

3. **Default Values**:
   - Use SQL functions when appropriate (e.g., GETUTCDATE() for timestamps)
   - Consider business logic implications when setting default values

4. **Primary Keys**:
   - Use meaningful column combinations for composite keys
   - Consider using surrogate keys for simpler relationships
   - Always use `nameof` operator when specifying key properties

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 
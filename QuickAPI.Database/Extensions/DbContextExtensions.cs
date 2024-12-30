using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QuickAPI.Database.Attributes;

namespace QuickAPI.Database.Extensions;

public static class DbContextExtensions
{
    public static void ApplyCustomDataAnnotations(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var type = entityType.ClrType;
            modelBuilder.CreateSqlDefaultValues(type);
            modelBuilder.CreateSqlComputedColumns(type);
            modelBuilder.CreateConstraintIndexesAndPrimaryKeys(type);
            entityType.RestrictCascadeDelete();
        }
    }

    private static void RestrictCascadeDelete(this IMutableEntityType type)
    {
        foreach (var foreignKey in type.GetForeignKeys())
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    private static void CreateSqlDefaultValues(this ModelBuilder modelBuilder, Type type)
    {
        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            var sqlDefaultAttribute = prop.GetCustomAttribute<SqlDefaultValueAttribute>();
            if (sqlDefaultAttribute is null)
            {
                continue;
            }

            var defaultValueBuilder = modelBuilder
                .Entity(type)
                .Property(prop.Name)
                .HasDefaultValueSql(sqlDefaultAttribute.Value);


            // defaultValueBuilder.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
            // defaultValueBuilder.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
            if (Nullable.GetUnderlyingType(prop.PropertyType) == null && prop.PropertyType != typeof(string))
            {
                defaultValueBuilder.Metadata.Sentinel = Activator.CreateInstance(prop.PropertyType);
            }
        }
    }

    private static void CreateSqlComputedColumns(this ModelBuilder modelBuilder, Type type)
    {
        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            var sqlComputedAttribute = prop.GetCustomAttribute<SqlComputedAttribute>();
            if (sqlComputedAttribute is null)
            {
                continue;
            }

            modelBuilder
                .Entity(type)
                .Property(prop.Name)
                .HasComputedColumnSql(sqlComputedAttribute.ComputeFunction, sqlComputedAttribute.Stored);
        }
    }

    private static void CreateConstraintIndexesAndPrimaryKeys(this ModelBuilder modelBuilder, Type type)
    {
        var constraintIndexAttributes = type.GetCustomAttributes<ConstraintIndexAttribute>()
            .ToList();
        var customPkIndexAttribute = type.GetCustomAttribute<CustomPrimaryKeyAttribute>();

        if (constraintIndexAttributes.Any(m => m.IsClustered))
        {
            foreach (var constraintIndexAttribute in constraintIndexAttributes)
            {
                modelBuilder.CreateConstraintIndex(constraintIndexAttribute, type);
            }

            modelBuilder.CreateCustomPrimaryKey(customPkIndexAttribute, type, isClustered: false);
            return;
        }

        if (constraintIndexAttributes.Count != 0 && constraintIndexAttributes.All(m => !m.IsClustered))
        {
            foreach (var constraintIndexAttribute in constraintIndexAttributes)
            {
                modelBuilder.CreateConstraintIndex(constraintIndexAttribute, type);
            }

            modelBuilder.CreateCustomPrimaryKey(customPkIndexAttribute, type);
            return;
        }

        if (constraintIndexAttributes.Count == 0)
        {
            modelBuilder.CreateCustomPrimaryKey(customPkIndexAttribute, type);
        }

        // var constraintIndexAttributes = type.GetCustomAttributes<ConstraintIndexAttribute>();
        // foreach (var constraintIndexAttribute in constraintIndexAttributes)
        // {
        //     modelBuilder.CreateConstraintIndex(constraintIndexAttribute, type);
        // }
    }

    private static void CreateCustomPrimaryKey(this ModelBuilder modelBuilder,
        CustomPrimaryKeyAttribute? customPrimaryKeyAttribute, Type type, bool isClustered = true)
    {
        if (customPrimaryKeyAttribute is null)
        {
            return;
        }

        const string prefix = "PK";
        var name = customPrimaryKeyAttribute.Name;
        if (string.IsNullOrEmpty(name))
        {
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            name = $"{prefix}_{tableName}";
        }

        var entityTypeBuilder = modelBuilder.Entity(type);
        entityTypeBuilder
            .HasKey(customPrimaryKeyAttribute.PropertyNames.ToArray())
            .IsClustered(isClustered)
            .HasName(name);
    }

    private static void CreateConstraintIndex(this ModelBuilder modelBuilder,
        ConstraintIndexAttribute constraintIndexAttribute, Type type)
    {
        var name = constraintIndexAttribute.Name;
        var prefix = constraintIndexAttribute.IsUnique ? "UQ" : "IX";
        if (string.IsNullOrEmpty(name))
        {
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
            var columnNames = string.Join("_", type.GetProperties()
                .Where(m => constraintIndexAttribute.PropertyNames.Contains(m.Name))
                .Select(m => m.GetCustomAttribute<ColumnAttribute>()?.Name ?? m.Name));

            name = $"{prefix}_{tableName}_{columnNames}";
        }

        var entityTypeBuilder = modelBuilder.Entity(type);
        if (constraintIndexAttribute.IsUnique)
        {
            entityTypeBuilder
                .HasAlternateKey(constraintIndexAttribute.PropertyNames.ToArray())
                .IsClustered(constraintIndexAttribute.IsClustered)
                .HasName(name);
            return;
        }

        

        var indexBuilder = entityTypeBuilder
            .HasIndex(constraintIndexAttribute.PropertyNames.ToArray())
            .IsClustered(constraintIndexAttribute.IsClustered)
            .HasDatabaseName(name);

        if (constraintIndexAttribute.IncludeProperties.Length == 0) 
            return;
        
        indexBuilder.IncludeProperties(constraintIndexAttribute.IncludeProperties);
    }

    private static void EntityLoop(this ModelBuilder builder, Action<IMutableEntityType> action)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            action(entityType);
        }
    }

    public static void RemoveOneToManyCascade(this ModelBuilder builder)
    {
        builder.EntityLoop(et => et.GetForeignKeys()
            .Where(fk => fk is { IsOwnership: false, DeleteBehavior: DeleteBehavior.Cascade })
            .ToList()
            .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict));
    }

    private static object GetDbSet(this DbContext context, Type type)
    {
        var dbSetMethod = context.GetType().GetMethod("Set", []);
        if (dbSetMethod is null)
        {
            throw new MissingMethodException("Set method could not be found.");
        }

        var typeSetMethod = dbSetMethod.MakeGenericMethod(type);
        var item = typeSetMethod.Invoke(context, []);

        if (item is null)
        {
            throw new MissingMemberException("DbSet object could not be found.");
        }

        return item;
    }


    public static object GetElement(this DbContext context, Type type, int id)
    {
        var set = context.GetDbSet(type);
        // var queryableExtensionsType = typeof(Queryable);
        //var queryableType = typeof(IQueryable<>).MakeGenericType(type);
        //var funcType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
        //var exprType = typeof(Expression<>).MakeGenericType(funcType);
        var firstOrDefaultMethods = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "FirstOrDefault");
        var firstOrDefaultMethod = firstOrDefaultMethods
            .FirstOrDefault(m => m.GetParameters().Length == 2
                                 && m.GetParameters().Any(p => p.ParameterType.BaseType == typeof(LambdaExpression)));
        if (firstOrDefaultMethod is null)
        {
            throw new MissingMethodException("FirstOrDefault method could not be found");
        }

        var genericMethod = firstOrDefaultMethod.MakeGenericMethod(type);
        if (genericMethod is null)
        {
            throw new MissingMethodException("FirstOrDefault method could not be made generic for given type");
        }


        var idProp = type.GetProperty("Id");

        if (idProp is null)
        {
            throw new MissingMemberException("Id property could not be made generic for given type");
        }

        var parameter = Expression.Parameter(type, "m");
        var property = Expression.Property(parameter, idProp);
        var rightSide = Expression.Constant(id);
        var operation = Expression.Equal(property, rightSide);
        var delegateType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
        var predicate = Expression.Lambda(delegateType, operation, parameter);

        var result = genericMethod.Invoke(set, [set, predicate]);
        if (result is null)
        {
            throw new NullReferenceException("Item could not be found");
        }

        return result;
    }
}
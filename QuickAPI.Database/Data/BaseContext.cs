using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuickAPI.Database.Attributes;
using QuickAPI.Database.DataModels;
using QuickAPI.Database.Extensions;
using QuickAPI.Database.Services.Core;

namespace QuickAPI.Database.Data;

public class BaseContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public BaseContext(DbContextOptions options,
        ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyCustomDataAnnotations();
        ApplyTenantQueryFilters(modelBuilder);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (!tenantId.HasValue || tenantId == Guid.Empty)
        {
            return;
        }

        // var departmentId = _departmentProvider.GetDepartmentId()
        foreach (var tenantModel in modelBuilder.Model.GetEntityTypes().Select(m => m.ClrType)
                     .Where(m => m.IsAssignableTo(typeof(ITenantModel))))
        {
            var expression = CreateTenantIdFilterExpression(tenantModel, tenantId.Value);
            modelBuilder.Entity(tenantModel, builder => { builder.HasQueryFilter(expression); });
        }
    }

    private LambdaExpression CreateTenantIdFilterExpression(Type tenantModelType, Guid tenantId)
    {
        //m => m.TenantId == _tenantId || m.TenantId == Guid.Empty
        var parameter = Expression.Parameter(tenantModelType, "m");
        var tenantIdProperty = Expression.Property(parameter, nameof(ITenantModel.TenantId));

        // var tenantId = _tenantProvider.GetCurrentTenantId();
        var tenantIdConstant = Expression.Constant(tenantId);
        var zeroConstant = Expression.Constant(Guid.Empty);

        var equalToTenantId = Expression.Equal(tenantIdProperty, tenantIdConstant);
        var equalToZero = Expression.Equal(tenantIdProperty, zeroConstant);

        var orExpression = Expression.OrElse(equalToTenantId, equalToZero);

        var lambdaExpression = Expression.Lambda(orExpression, parameter);
        return lambdaExpression;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        SetCreatedAndModifiedValues();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        SetCreatedAndModifiedValues();
        return base.SaveChanges();
    }

    private void SetCreatedAndModifiedValues()
    {
        var addedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .ToList();

        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        var currentUser = _tenantProvider.GetCurrentUser();

        foreach (var entry in addedEntries)
        {
            // Set CreatedAt for BaseModel entities
            SetCreatedValues(entry, currentUser);
        }

        foreach (var entry in modifiedEntries)
        {
            // Set ModifiedAt for BaseModel entities
            SetModifiedValues(entry, currentUser);
        }

        // Set TenantId for ITenantEntity entities
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (!tenantId.HasValue)
            return;
        var tenantEntries = addedEntries.Where(e => e.Entity is ITenantModel);
        foreach (var entry in tenantEntries)
        {
            ((ITenantModel)entry.Entity).TenantId = tenantId.Value;
        }
    }

    private static void SetModifiedValues(EntityEntry entry, string? currentUser)
    {
        if (entry.Entity is not BaseModel baseModel)
            return;
        baseModel.ModifiedAt = DateTimeOffset.UtcNow;
        baseModel.ModifiedBy = currentUser ?? string.Empty;
    }

    private static void SetCreatedValues(EntityEntry entry, string? currentUser)
    {
        if (entry.Entity is not BaseModel baseModel)
            return;
        baseModel.CreatedAt = DateTimeOffset.UtcNow;
        baseModel.CreatedBy = currentUser ?? string.Empty;
    }
}
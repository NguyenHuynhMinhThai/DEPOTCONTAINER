using System.Linq.Expressions;
using DEPOTCONTAINER.Data;
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DEPOTCONTAINER.Repositories;

/// <summary>
/// Generic Repository triển khai trên Entity Framework Core.
/// Áp dụng Generic Pattern và sử dụng Predicate/Func qua Expression Tree.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly DepotDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(DepotDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // ============ Read ============
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken);
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    // ============ Write ============
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
            entity.CreatedAt = now;
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
            entity.UpdatedAt = now;
        _dbSet.UpdateRange(entities);
    }

    public virtual void Remove(T entity)
    {
        // Soft delete - chỉ đánh dấu IsDeleted
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = now;
        }
        _dbSet.UpdateRange(entities);
    }

    // ============ Paging ============
    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        IQueryable<T> query = _dbSet;

        if (predicate != null)
            query = query.Where(predicate);

        int totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
            query = orderBy(query);
        else
            query = query.OrderByDescending(e => e.Id);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
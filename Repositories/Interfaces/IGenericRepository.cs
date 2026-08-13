using System.Linq.Expressions;
using DEPOTCONTAINER.Models.Entities;

namespace DEPOTCONTAINER.Repositories.Interfaces;

/// <summary>
/// Generic Repository Pattern.
/// Cung cấp các thao tác CRUD cơ bản cho mọi entity trong hệ thống.
/// Áp dụng Generic + SOLID (Dependency Inversion).
/// </summary>
/// <typeparam name="T">Entity kế thừa BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    // ============ Read ============
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // ============ Write ============
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Remove(T entity); // Soft delete

    void RemoveRange(IEnumerable<T> entities);

    // ============ Paging & Query ============
    Task<Models.DTOs.PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default);
}
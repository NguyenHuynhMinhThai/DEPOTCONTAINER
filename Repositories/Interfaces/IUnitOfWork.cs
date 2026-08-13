using DEPOTCONTAINER.Data;
using DEPOTCONTAINER.Repositories.Interfaces;

namespace DEPOTCONTAINER.Repositories.Interfaces;

/// <summary>
/// Unit of Work Pattern - gom tất cả repository + DbContext vào 1 đối tượng.
/// Giúp quản lý transaction và đảm bảo tất cả thay đổi được commit cùng lúc.
/// Áp dụng Pattern: Unit of Work + Repository.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IContainerRepository Containers { get; }
    IBlockRepository Blocks { get; }
    ILineOperatorRepository LineOperators { get; }
    ICustomerRepository Customers { get; }
    IContainerMovementRepository ContainerMovements { get; }
    IReleaseOrderRepository ReleaseOrders { get; }

    /// <summary>Lưu tất cả thay đổi vào database</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Bắt đầu transaction</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commit transaction</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rollback transaction</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
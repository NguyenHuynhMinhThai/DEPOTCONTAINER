using DEPOTCONTAINER.Data;
using DEPOTCONTAINER.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DEPOTCONTAINER.Repositories;

/// <summary>
/// Triển khai Unit of Work - quản lý tất cả repositories + transaction.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DepotDbContext _context;
    private bool _disposed;

    /// <summary>Expose DbContext cho các trường hợp cần truy cập trực tiếp (ví dụ: generate layout).</summary>
    public DepotDbContext Context => _context;

    public UnitOfWork(DepotDbContext context)
    {
        _context = context;
        Containers = new ContainerRepository(context);
        Blocks = new BlockRepository(context);
        LineOperators = new LineOperatorRepository(context);
        Customers = new CustomerRepository(context);
        ContainerMovements = new ContainerMovementRepository(context);
        ReleaseOrders = new ReleaseOrderRepository(context);
    }

    public IContainerRepository Containers { get; private set; }
    public IBlockRepository Blocks { get; private set; }
    public ILineOperatorRepository LineOperators { get; private set; }
    public ICustomerRepository Customers { get; private set; }
    public IContainerMovementRepository ContainerMovements { get; private set; }
    public IReleaseOrderRepository ReleaseOrders { get; private set; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}

/// <summary>
/// Helper để tạo DbContext tạm thời cho các tác vụ ngoài DI (ví dụ trong test).
/// </summary>
public static class DbContextFactory
{
    public static DepotDbContext Create()
    {
        var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DepotDbContext>()
            .UseMySql("server=localhost;database=depotdb;user=root;password=root",
                Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect("server=localhost;database=depotdb;user=root;password=root"))
            .Options;
        return new DepotDbContext(options);
    }
}
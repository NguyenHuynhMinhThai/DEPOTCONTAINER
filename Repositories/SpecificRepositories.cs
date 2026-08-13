using DEPOTCONTAINER.Data;
using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DEPOTCONTAINER.Repositories;

/// <summary>
/// Repository cho Container - triển khai các phương thức đặc thù.
/// </summary>
public class ContainerRepository : GenericRepository<Container>, IContainerRepository
{
    public ContainerRepository(DepotDbContext context) : base(context) { }

    public async Task<Container?> GetByContainerNumberAsync(string containerNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ContainerNumber == containerNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Container>> GetContainersInBlockAsync(int blockId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.CurrentBlockId == blockId)
            .Include(c => c.CurrentBay)
            .Include(c => c.CurrentRow)
            .Include(c => c.CurrentTier)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Container>> GetContainersByLineOperatorAsync(int lineOperatorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.LineOperatorId == lineOperatorId)
            .Include(c => c.LineOperator)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Container?> GetContainerWithMovementsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Movements)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}

/// <summary>
/// Repository cho Block.
/// </summary>
public class BlockRepository : GenericRepository<Block>, IBlockRepository
{
    public BlockRepository(DepotDbContext context) : base(context) { }

    public async Task<Block?> GetBlockWithLayoutAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Bays)
                .ThenInclude(bay => bay.Rows)
                    .ThenInclude(r => r.Tiers)
            .Include(b => b.Containers)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Block?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Code == code, cancellationToken);
    }
}

/// <summary>
/// Repository cho LineOperator.
/// </summary>
public class LineOperatorRepository : GenericRepository<LineOperator>, ILineOperatorRepository
{
    public LineOperatorRepository(DepotDbContext context) : base(context) { }

    public async Task<LineOperator?> GetByOwnerCodeAsync(string ownerCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.OwnerCode == ownerCode, cancellationToken);
    }
}

/// <summary>
/// Repository cho Customer.
/// </summary>
public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(DepotDbContext context) : base(context) { }

    public async Task<Customer?> GetByTaxCodeAsync(string taxCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.TaxCode == taxCode, cancellationToken);
    }
}

/// <summary>
/// Repository cho ContainerMovement.
/// </summary>
public class ContainerMovementRepository : GenericRepository<ContainerMovement>, IContainerMovementRepository
{
    public ContainerMovementRepository(DepotDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ContainerMovement>> GetMovementsByContainerAsync(int containerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.ContainerId == containerId)
            .OrderByDescending(m => m.MovementDate)
            .Include(m => m.ToBlock)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContainerMovement>> GetMovementsByTypeAsync(Models.Enums.MovementType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.MovementType == type)
            .OrderByDescending(m => m.MovementDate)
            .Include(m => m.Container)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Repository cho ReleaseOrder.
/// </summary>
public class ReleaseOrderRepository : GenericRepository<ReleaseOrder>, IReleaseOrderRepository
{
    public ReleaseOrderRepository(DepotDbContext context) : base(context) { }

    public async Task<ReleaseOrder?> GetOrderWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.LineOperator)
            .Include(r => r.Customer)
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<ReleaseOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.OrderNumber == orderNumber, cancellationToken);
    }
}
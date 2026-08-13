using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Repositories.Interfaces;

namespace DEPOTCONTAINER.Repositories.Interfaces;

/// <summary>
/// Repository riêng cho Container - bổ sung các phương thức đặc thù.
/// Kế thừa IGenericRepository, áp dụng nguyên lý Interface Segregation (SOLID).
/// </summary>
public interface IContainerRepository : IGenericRepository<Container>
{
    Task<Container?> GetByContainerNumberAsync(string containerNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Container>> GetContainersInBlockAsync(int blockId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Container>> GetContainersByLineOperatorAsync(int lineOperatorId, CancellationToken cancellationToken = default);

    Task<Container?> GetContainerWithMovementsAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository cho Block + Bay/Row/Tier.
/// </summary>
public interface IBlockRepository : IGenericRepository<Block>
{
    Task<Block?> GetBlockWithLayoutAsync(int id, CancellationToken cancellationToken = default);
    Task<Block?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository cho LineOperator.
/// </summary>
public interface ILineOperatorRepository : IGenericRepository<LineOperator>
{
    Task<LineOperator?> GetByOwnerCodeAsync(string ownerCode, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository cho Customer.
/// </summary>
public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByTaxCodeAsync(string taxCode, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository cho Movement.
/// </summary>
public interface IContainerMovementRepository : IGenericRepository<ContainerMovement>
{
    Task<IReadOnlyList<ContainerMovement>> GetMovementsByContainerAsync(int containerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContainerMovement>> GetMovementsByTypeAsync(Models.Enums.MovementType type, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository cho ReleaseOrder.
/// </summary>
public interface IReleaseOrderRepository : IGenericRepository<ReleaseOrder>
{
    Task<ReleaseOrder?> GetOrderWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<ReleaseOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
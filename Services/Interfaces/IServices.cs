using DEPOTCONTAINER.Models.DTOs;

namespace DEPOTCONTAINER.Services.Interfaces;

/// <summary>
/// Service cơ sở cho tất cả service khác.
/// Cung cấp helper trả về ApiResponse thống nhất.
/// </summary>
public interface IBaseService
{
    ApiResponse<T> Success<T>(T data, string message = "Thao tác thành công");

    ApiResponse<T> Failure<T>(string message, List<string>? errors = null);
}

/// <summary>
/// Service cho Container.
/// </summary>
public interface IContainerService
{
    Task<ApiResponse<PagedResult<ContainerDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);

    Task<ApiResponse<ContainerDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiResponse<ContainerDto>> CreateAsync(CreateContainerDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<ContainerDto>> UpdateAsync(int id, UpdateContainerDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiResponse<ContainerDto>> GetByContainerNumberAsync(string containerNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate Container Number theo Modulo 11.
    /// </summary>
    Task<ApiResponse<bool>> ValidateContainerNumberAsync(string containerNumber);

    /// <summary>
    /// Assign container vào 1 vị trí cụ thể trong block.
    /// </summary>
    Task<ApiResponse<ContainerDto>> AssignLocationAsync(int containerId, int? blockId, int? bayId, int? rowId, int? tierId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service cho Block.
/// </summary>
public interface IBlockService
{
    Task<ApiResponse<PagedResult<BlockDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);

    Task<ApiResponse<BlockDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiResponse<BlockDto>> CreateAsync(CreateBlockDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<BlockDto>> UpdateAsync(int id, CreateBlockDto dto, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<ApiResponse<BlockLayoutDto>> GetBlockLayoutAsync(int blockId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tự động sinh Bay/Row/Tier theo MaxBays/MaxRows/MaxTiers.
    /// </summary>
    Task<ApiResponse<bool>> GenerateLayoutAsync(int blockId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service cho LineOperator.
/// </summary>
public interface ILineOperatorService
{
    Task<ApiResponse<PagedResult<LineOperatorDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);
    Task<ApiResponse<LineOperatorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<LineOperatorDto>> CreateAsync(LineOperatorDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<LineOperatorDto>> UpdateAsync(int id, LineOperatorDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service cho Customer.
/// </summary>
public interface ICustomerService
{
    Task<ApiResponse<PagedResult<CustomerDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);
    Task<ApiResponse<CustomerDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<CustomerDto>> CreateAsync(CustomerDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<CustomerDto>> UpdateAsync(int id, CustomerDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service cho ContainerMovement.
/// </summary>
public interface IContainerMovementService
{
    Task<ApiResponse<PagedResult<ContainerMovementDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);
    Task<ApiResponse<ContainerMovementDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ContainerMovementDto>> CreateAsync(CreateMovementDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy lịch sử di chuyển của 1 container.
    /// </summary>
    Task<ApiResponse<IReadOnlyList<ContainerMovementDto>>> GetMovementsByContainerAsync(int containerId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service cho ReleaseOrder.
/// </summary>
public interface IReleaseOrderService
{
    Task<ApiResponse<PagedResult<ReleaseOrderDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);
    Task<ApiResponse<ReleaseOrderDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<ReleaseOrderDto>> CreateAsync(CreateReleaseOrderDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<ReleaseOrderDto>> UpdateStatusAsync(int id, Models.Enums.ReleaseOrderStatus status, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Thực hiện lệnh giao container (xuất container ra khỏi bãi theo ReleaseOrder).
    /// </summary>
    Task<ApiResponse<ContainerMovementDto>> ExecuteReleaseAsync(int releaseOrderId, int containerId, string vehicle, string? driverName, CancellationToken cancellationToken = default);
}
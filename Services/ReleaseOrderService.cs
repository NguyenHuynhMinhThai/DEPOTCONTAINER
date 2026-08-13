using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Repositories;
using DEPOTCONTAINER.Repositories.Interfaces;
using DEPOTCONTAINER.Services.Interfaces;

namespace DEPOTCONTAINER.Services;

/// <summary>
/// Service cho ReleaseOrder (lệnh giao container).
/// Theo đề bài: depot KHÔNG ĐƯỢC tự ý giao container ra ngoài, phải có lệnh này.
/// </summary>
public class ReleaseOrderService : BaseService, IReleaseOrderService
{
    private readonly IUnitOfWork _uow;

    public ReleaseOrderService(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResponse<PagedResult<ReleaseOrderDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
    {
        System.Linq.Expressions.Expression<Func<ReleaseOrder, bool>>? predicate = null;
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            predicate = r => r.OrderNumber.ToLower().Contains(search)
                         || (r.LineOperator != null && r.LineOperator.Name.ToLower().Contains(search))
                         || (r.Customer != null && r.Customer.Name.ToLower().Contains(search));
        }

        System.Func<System.Linq.IQueryable<ReleaseOrder>, System.Linq.IOrderedQueryable<ReleaseOrder>>? orderBy = parameters.SortBy?.ToLower() switch
        {
            "order" => q => parameters.SortDescending ? q.OrderByDescending(r => r.OrderNumber) : q.OrderBy(r => r.OrderNumber),
            "valid" => q => parameters.SortDescending ? q.OrderByDescending(r => r.ValidUntil) : q.OrderBy(r => r.ValidUntil),
            "status" => q => parameters.SortDescending ? q.OrderByDescending(r => r.Status) : q.OrderBy(r => r.Status),
            _ => q => q.OrderByDescending(r => r.Id)
        };

        var paged = await _uow.ReleaseOrders.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize, predicate, orderBy, cancellationToken);

        var items = paged.Items.Select(ReleaseOrderDto.FromEntity).ToList();
        var result = new PagedResult<ReleaseOrderDto>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
        return Success(result);
    }

    public async Task<ApiResponse<ReleaseOrderDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.ReleaseOrders.GetOrderWithDetailsAsync(id, cancellationToken);
        if (entity == null) return Failure<ReleaseOrderDto>($"Không tìm thấy ReleaseOrder Id={id}");
        return Success(ReleaseOrderDto.FromEntity(entity));
    }

    public async Task<ApiResponse<ReleaseOrderDto>> CreateAsync(CreateReleaseOrderDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.ReleaseOrders.GetByOrderNumberAsync(dto.OrderNumber, cancellationToken);
        if (existing != null)
            return Failure<ReleaseOrderDto>($"Số lệnh '{dto.OrderNumber}' đã tồn tại");

        var lineOperator = await _uow.LineOperators.GetByIdAsync(dto.LineOperatorId, cancellationToken);
        if (lineOperator == null)
            return Failure<ReleaseOrderDto>("Line Operator không tồn tại");

        var customer = await _uow.Customers.GetByIdAsync(dto.CustomerId, cancellationToken);
        if (customer == null)
            return Failure<ReleaseOrderDto>("Customer không tồn tại");

        await _uow.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = new ReleaseOrder
            {
                OrderNumber = dto.OrderNumber,
                LineOperatorId = dto.LineOperatorId,
                CustomerId = dto.CustomerId,
                ValidUntil = dto.ValidUntil,
                ExportVessel = dto.ExportVessel,
                ExportDate = dto.ExportDate,
                Description = dto.Description,
                Status = ReleaseOrderStatus.New
            };
            await _uow.ReleaseOrders.AddAsync(order, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            foreach (var d in dto.Details)
            {
                var detail = new ReleaseOrderDetail
                {
                    ReleaseOrderId = order.Id,
                    ContainerSize = d.ContainerSize,
                    ContainerType = d.ContainerType,
                    Quantity = d.Quantity,
                    Note = d.Note
                };
                await ((UnitOfWork)_uow).Context.ReleaseOrderDetails.AddAsync(detail, cancellationToken);
            }
            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);

            var withDetails = await _uow.ReleaseOrders.GetOrderWithDetailsAsync(order.Id, cancellationToken);
            return Success(ReleaseOrderDto.FromEntity(withDetails!), "Tạo lệnh giao container thành công");
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            return Failure<ReleaseOrderDto>($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ReleaseOrderDto>> UpdateStatusAsync(int id, ReleaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.ReleaseOrders.GetOrderWithDetailsAsync(id, cancellationToken);
        if (entity == null) return Failure<ReleaseOrderDto>($"Không tìm thấy ReleaseOrder Id={id}");

        entity.Status = status;
        _uow.ReleaseOrders.Update(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(ReleaseOrderDto.FromEntity(entity), "Cập nhật trạng thái thành công");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.ReleaseOrders.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<bool>($"Không tìm thấy ReleaseOrder Id={id}");

        if (entity.Status == ReleaseOrderStatus.InProgress)
            return Failure<bool>("Không thể xóa lệnh đang thực hiện");

        _uow.ReleaseOrders.Remove(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(true, "Xóa thành công");
    }

    /// <summary>
    /// Thực hiện lệnh giao container: xuất container ra khỏi bãi theo ReleaseOrder.
    /// Container phải có trong bãi, đúng loại, và lệnh phải còn hiệu lực.
    /// </summary>
    public async Task<ApiResponse<ContainerMovementDto>> ExecuteReleaseAsync(int releaseOrderId, int containerId, string vehicle, string? driverName, CancellationToken cancellationToken = default)
    {
        var order = await _uow.ReleaseOrders.GetOrderWithDetailsAsync(releaseOrderId, cancellationToken);
        if (order == null) return Failure<ContainerMovementDto>($"Không tìm thấy ReleaseOrder Id={releaseOrderId}");

        if (order.Status != ReleaseOrderStatus.New && order.Status != ReleaseOrderStatus.InProgress)
            return Failure<ContainerMovementDto>($"Lệnh đang ở trạng thái '{order.Status}', không thể thực hiện");

        if (order.ValidUntil < DateTime.UtcNow)
            return Failure<ContainerMovementDto>("Lệnh đã hết hạn");

        var container = await _uow.Containers.GetByIdAsync(containerId, cancellationToken);
        if (container == null) return Failure<ContainerMovementDto>("Không tìm thấy container");

        if (!container.IsInYard)
            return Failure<ContainerMovementDto>("Container không có trong bãi");

        await _uow.BeginTransactionAsync(cancellationToken);
        try
        {
            var movement = new ContainerMovement
            {
                ContainerId = containerId,
                MovementType = MovementType.Out,
                MovementDate = DateTime.UtcNow,
                Vehicle = vehicle,
                DriverName = driverName,
                ReleaseOrderId = releaseOrderId,
                Note = $"Giao theo lệnh {order.OrderNumber}"
            };
            await _uow.ContainerMovements.AddAsync(movement, cancellationToken);

            // Cập nhật container
            container.IsInYard = false;
            container.CurrentBlockId = null;
            container.CurrentBayId = null;
            container.CurrentRowId = null;
            container.CurrentTierId = null;
            _uow.Containers.Update(container);

            // Tìm detail tương ứng để cập nhật số lượng đã giao
            var matchingDetail = order.Details.FirstOrDefault(d =>
                d.ContainerSize == container.Size && d.ContainerType == container.ContainerType);

            if (matchingDetail != null)
            {
                if (matchingDetail.DeliveredQuantity < matchingDetail.Quantity)
                {
                    matchingDetail.DeliveredQuantity++;
                    ((UnitOfWork)_uow).Context.ReleaseOrderDetails.Update(matchingDetail);
                }
            }

            // Cập nhật status nếu tất cả đã giao đủ
            var allCompleted = order.Details.All(d => d.DeliveredQuantity >= d.Quantity);
            if (allCompleted)
                order.Status = ReleaseOrderStatus.Completed;
            else if (order.Status == ReleaseOrderStatus.New)
                order.Status = ReleaseOrderStatus.InProgress;

            _uow.ReleaseOrders.Update(order);
            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);

            return Success(ContainerMovementDto.FromEntity(movement), "Giao container thành công");
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            return Failure<ContainerMovementDto>($"Lỗi: {ex.Message}");
        }
    }
}
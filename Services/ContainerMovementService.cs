using System.Linq.Expressions;
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Repositories.Interfaces;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DEPOTCONTAINER.Services;

/// <summary>
/// Service cho ContainerMovement - vòng đời vào/ra bãi của container.
/// Đây là phần nghiệp vụ quan trọng nhất theo yêu cầu đề bài.
/// </summary>
public class ContainerMovementService : BaseService, IContainerMovementService
{
    private readonly IUnitOfWork _uow;

    public ContainerMovementService(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResponse<PagedResult<ContainerMovementDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
    {
        Expression<Func<ContainerMovement, bool>>? predicate = null;
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            predicate = m => m.Vehicle.ToLower().Contains(search)
                         || (m.Container != null && m.Container.ContainerNumber.ToLower().Contains(search));
        }

        Func<IQueryable<ContainerMovement>, IOrderedQueryable<ContainerMovement>>? orderBy = parameters.SortBy?.ToLower() switch
        {
            "date" => q => parameters.SortDescending ? q.OrderByDescending(m => m.MovementDate) : q.OrderBy(m => m.MovementDate),
            "type" => q => parameters.SortDescending ? q.OrderByDescending(m => m.MovementType) : q.OrderBy(m => m.MovementType),
            _ => q => q.OrderByDescending(m => m.MovementDate)
        };

        var paged = await _uow.ContainerMovements.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize, predicate, orderBy, cancellationToken);

        var items = paged.Items.Select(ContainerMovementDto.FromEntity).ToList();
        var result = new PagedResult<ContainerMovementDto>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
        return Success(result);
    }

    public async Task<ApiResponse<ContainerMovementDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.ContainerMovements.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<ContainerMovementDto>($"Không tìm thấy movement Id={id}");
        return Success(ContainerMovementDto.FromEntity(entity));
    }

    public async Task<ApiResponse<ContainerMovementDto>> CreateAsync(CreateMovementDto dto, CancellationToken cancellationToken = default)
    {
        var container = await _uow.Containers.GetByIdAsync(dto.ContainerId, cancellationToken);
        if (container == null)
            return Failure<ContainerMovementDto>("Không tìm thấy container");

        await _uow.BeginTransactionAsync(cancellationToken);
        try
        {
            var movement = new ContainerMovement
            {
                ContainerId = dto.ContainerId,
                MovementType = dto.MovementType,
                MovementDate = DateTime.UtcNow,
                Vehicle = dto.Vehicle,
                VehicleType = dto.VehicleType,
                SealNumber = dto.SealNumber,
                ToBlockId = dto.ToBlockId,
                DriverName = dto.DriverName,
                DriverId = dto.DriverId,
                ReleaseOrderId = dto.ReleaseOrderId,
                Note = dto.Note
            };

            await _uow.ContainerMovements.AddAsync(movement, cancellationToken);

            // Cập nhật trạng thái container tương ứng với movement
            if (dto.MovementType == MovementType.In)
            {
                container.IsInYard = true;
                if (dto.ToBlockId.HasValue) container.CurrentBlockId = dto.ToBlockId;
            }
            else if (dto.MovementType == MovementType.Out)
            {
                container.IsInYard = false;
                container.CurrentBlockId = null;
                container.CurrentBayId = null;
                container.CurrentRowId = null;
                container.CurrentTierId = null;
            }
            _uow.Containers.Update(container);

            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);

            return Success(ContainerMovementDto.FromEntity(movement), "Tạo movement thành công");
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            return Failure<ContainerMovementDto>($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IReadOnlyList<ContainerMovementDto>>> GetMovementsByContainerAsync(int containerId, CancellationToken cancellationToken = default)
    {
        var movements = await _uow.ContainerMovements.GetMovementsByContainerAsync(containerId, cancellationToken);
        var dtos = movements.Select(ContainerMovementDto.FromEntity).ToList();
        return Success<IReadOnlyList<ContainerMovementDto>>(dtos);
    }
}
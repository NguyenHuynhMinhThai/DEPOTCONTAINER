using System.Linq.Expressions;
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Repositories;
using DEPOTCONTAINER.Repositories.Interfaces;
using DEPOTCONTAINER.Services.Interfaces;
using DEPOTCONTAINER.Validators;
using Microsoft.EntityFrameworkCore;

namespace DEPOTCONTAINER.Services;

/// <summary>
/// Base service cung cấp helper trả về ApiResponse.
/// </summary>
public abstract class BaseService : IBaseService
{
    public ApiResponse<T> Success<T>(T data, string message = "Thao tác thành công")
        => ApiResponse<T>.Ok(data, message);

    public ApiResponse<T> Failure<T>(string message, List<string>? errors = null)
        => ApiResponse<T>.Fail(message, errors);
}

/// <summary>
/// Service cho Container - chứa toàn bộ nghiệp vụ liên quan đến Container.
/// </summary>
public class ContainerService : BaseService, IContainerService
{
    private readonly IUnitOfWork _uow;

    /// <summary>Action delegate - dùng để log</summary>
    private readonly Action<string> _logAction;

    public ContainerService(IUnitOfWork uow)
    {
        _uow = uow;
        _logAction = msg => Console.WriteLine($"[ContainerService] {DateTime.UtcNow:HH:mm:ss} - {msg}");
    }

    public async Task<ApiResponse<PagedResult<ContainerDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
    {
        Expression<Func<Container, bool>>? predicate = null;
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            predicate = c => c.ContainerNumber.ToLower().Contains(search)
                         || c.IsoCode.ToLower().Contains(search);
        }

        Func<IQueryable<Container>, IOrderedQueryable<Container>>? orderBy = parameters.SortBy?.ToLower() switch
        {
            "number" => q => parameters.SortDescending ? q.OrderByDescending(c => c.ContainerNumber) : q.OrderBy(c => c.ContainerNumber),
            "type" => q => parameters.SortDescending ? q.OrderByDescending(c => c.ContainerType) : q.OrderBy(c => c.ContainerType),
            "size" => q => parameters.SortDescending ? q.OrderByDescending(c => c.Size) : q.OrderBy(c => c.Size),
            _ => q => q.OrderByDescending(c => c.Id)
        };

        var paged = await _uow.Containers.GetPagedAsync(
            parameters.PageNumber,
            parameters.PageSize,
            predicate,
            orderBy,
            cancellationToken);

        var items = paged.Items.Select(ContainerDto.FromEntity).ToList();
        var result = new PagedResult<ContainerDto>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
        return Success(result);
    }

    public async Task<ApiResponse<ContainerDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var container = await _uow.Containers.GetByIdAsync(id, cancellationToken);
        if (container == null)
            return Failure<ContainerDto>($"Không tìm thấy container với Id={id}");

        // Load navigation
        var withNav = await _uow.Containers.GetContainerWithMovementsAsync(id, cancellationToken);
        return Success(ContainerDto.FromEntity(withNav ?? container));
    }

    public async Task<ApiResponse<ContainerDto>> CreateAsync(CreateContainerDto dto, CancellationToken cancellationToken = default)
    {
        var (isValid, errorMessage) = ContainerNumberValidator.ValidateWithMessage(dto.ContainerNumber);
        if (!isValid)
            return Failure<ContainerDto>(errorMessage ?? "Số container không hợp lệ");

        var existing = await _uow.Containers.GetByContainerNumberAsync(dto.ContainerNumber, cancellationToken);
        if (existing != null)
            return Failure<ContainerDto>($"Số container '{dto.ContainerNumber}' đã tồn tại");

        var container = new Container
        {
            ContainerNumber = dto.ContainerNumber.ToUpper(),
            ContainerType = dto.ContainerType,
            IsoCode = dto.IsoCode,
            Size = dto.Size,
            MaxWeight = dto.MaxWeight,
            TareWeight = dto.TareWeight,
            ManufactureDate = dto.ManufactureDate,
            LineOperatorId = dto.LineOperatorId,
            Condition = dto.Condition,
            Category = dto.Category,
            DamageDescription = dto.DamageDescription
        };

        await _uow.Containers.AddAsync(container, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logAction($"Created container {container.ContainerNumber}");
        return Success(ContainerDto.FromEntity(container), "Tạo container thành công");
    }

    public async Task<ApiResponse<ContainerDto>> UpdateAsync(int id, UpdateContainerDto dto, CancellationToken cancellationToken = default)
    {
        var container = await _uow.Containers.GetByIdAsync(id, cancellationToken);
        if (container == null)
            return Failure<ContainerDto>($"Không tìm thấy container với Id={id}");

        container.ContainerType = dto.ContainerType;
        container.IsoCode = dto.IsoCode;
        container.Size = dto.Size;
        container.MaxWeight = dto.MaxWeight;
        container.TareWeight = dto.TareWeight;
        container.ManufactureDate = dto.ManufactureDate;
        container.LineOperatorId = dto.LineOperatorId;
        container.Condition = dto.Condition;
        container.Category = dto.Category;
        container.DamageDescription = dto.DamageDescription;

        _uow.Containers.Update(container);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(ContainerDto.FromEntity(container), "Cập nhật thành công");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var container = await _uow.Containers.GetByIdAsync(id, cancellationToken);
        if (container == null)
            return Failure<bool>($"Không tìm thấy container với Id={id}");

        _uow.Containers.Remove(container);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(true, "Xóa container thành công");
    }

    public async Task<ApiResponse<ContainerDto>> GetByContainerNumberAsync(string containerNumber, CancellationToken cancellationToken = default)
    {
        var container = await _uow.Containers.GetByContainerNumberAsync(containerNumber.ToUpper(), cancellationToken);
        if (container == null)
            return Failure<ContainerDto>($"Không tìm thấy container với số '{containerNumber}'");
        return Success(ContainerDto.FromEntity(container));
    }

    public Task<ApiResponse<bool>> ValidateContainerNumberAsync(string containerNumber)
    {
        var (isValid, errorMessage) = ContainerNumberValidator.ValidateWithMessage(containerNumber);
        return Task.FromResult(isValid
            ? Success(true, "Số container hợp lệ")
            : Failure<bool>(errorMessage ?? "Số container không hợp lệ"));
    }

    public async Task<ApiResponse<ContainerDto>> AssignLocationAsync(int containerId, int? blockId, int? bayId, int? rowId, int? tierId, CancellationToken cancellationToken = default)
    {
        var container = await _uow.Containers.GetByIdAsync(containerId, cancellationToken);
        if (container == null)
            return Failure<ContainerDto>("Không tìm thấy container");

        container.CurrentBlockId = blockId;
        container.CurrentBayId = bayId;
        container.CurrentRowId = rowId;
        container.CurrentTierId = tierId;
        container.IsInYard = blockId.HasValue;

        _uow.Containers.Update(container);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(ContainerDto.FromEntity(container), "Cập nhật vị trí thành công");
    }
}

/// <summary>
/// Service cho Block.
/// </summary>
public class BlockService : BaseService, IBlockService
{
    private readonly IUnitOfWork _uow;

    public BlockService(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResponse<PagedResult<BlockDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
    {
        Expression<Func<Block, bool>>? predicate = null;
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            predicate = b => b.Code.ToLower().Contains(search) || b.Name.ToLower().Contains(search);
        }

        Func<IQueryable<Block>, IOrderedQueryable<Block>>? orderBy = parameters.SortBy?.ToLower() switch
        {
            "code" => q => parameters.SortDescending ? q.OrderByDescending(b => b.Code) : q.OrderBy(b => b.Code),
            "name" => q => parameters.SortDescending ? q.OrderByDescending(b => b.Name) : q.OrderBy(b => b.Name),
            _ => q => q.OrderBy(b => b.Code)
        };

        var paged = await _uow.Blocks.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize, predicate, orderBy, cancellationToken);

        var items = paged.Items.Select(BlockDto.FromEntity).ToList();
        var result = new PagedResult<BlockDto>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
        return Success(result);
    }

    public async Task<ApiResponse<BlockDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var block = await _uow.Blocks.GetBlockWithLayoutAsync(id, cancellationToken);
        if (block == null)
            return Failure<BlockDto>($"Không tìm thấy block với Id={id}");
        return Success(BlockDto.FromEntity(block));
    }

    public async Task<ApiResponse<BlockDto>> CreateAsync(CreateBlockDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Blocks.GetByCodeAsync(dto.Code, cancellationToken);
        if (existing != null)
            return Failure<BlockDto>($"Mã block '{dto.Code}' đã tồn tại");

        var block = new Block
        {
            Code = dto.Code,
            Name = dto.Name,
            BlockType = dto.BlockType,
            MaxBays = dto.MaxBays,
            MaxRows = dto.MaxRows,
            MaxTiers = dto.MaxTiers,
            MaxContainerSize = dto.MaxContainerSize,
            Description = dto.Description,
            IsActive = true
        };

        await _uow.Blocks.AddAsync(block, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        if (block.BlockType == BlockType.Physical)
        {
            await GenerateLayoutInternalAsync(block, cancellationToken);
        }

        return Success(BlockDto.FromEntity(block), "Tạo block thành công");
    }

    public async Task<ApiResponse<BlockDto>> UpdateAsync(int id, CreateBlockDto dto, CancellationToken cancellationToken = default)
    {
        var block = await _uow.Blocks.GetByIdAsync(id, cancellationToken);
        if (block == null)
            return Failure<BlockDto>($"Không tìm thấy block với Id={id}");

        block.Code = dto.Code;
        block.Name = dto.Name;
        block.MaxBays = dto.MaxBays;
        block.MaxRows = dto.MaxRows;
        block.MaxTiers = dto.MaxTiers;
        block.MaxContainerSize = dto.MaxContainerSize;
        block.Description = dto.Description;

        _uow.Blocks.Update(block);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(BlockDto.FromEntity(block), "Cập nhật block thành công");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var block = await _uow.Blocks.GetByIdAsync(id, cancellationToken);
        if (block == null)
            return Failure<bool>($"Không tìm thấy block với Id={id}");

        var containersInBlock = await _uow.Containers.GetContainersInBlockAsync(id, cancellationToken);
        if (containersInBlock.Any())
            return Failure<bool>($"Không thể xóa block vì đang có {containersInBlock.Count} container");

        _uow.Blocks.Remove(block);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(true, "Xóa block thành công");
    }

    public async Task<ApiResponse<BlockLayoutDto>> GetBlockLayoutAsync(int blockId, CancellationToken cancellationToken = default)
    {
        var block = await _uow.Blocks.GetBlockWithLayoutAsync(blockId, cancellationToken);
        if (block == null)
            return Failure<BlockLayoutDto>("Không tìm thấy block");

        var layout = new BlockLayoutDto
        {
            BlockId = block.Id,
            BlockCode = block.Code,
            Bays = block.Bays
                .OrderBy(b => b.BayNumber)
                .Select(b => new BayLayoutDto
                {
                    BayId = b.Id,
                    BayNumber = b.BayNumber,
                    ContainerSize = b.ContainerSize,
                    Rows = b.Rows
                        .OrderBy(r => r.RowNumber)
                        .Select(r => new RowLayoutDto
                        {
                            RowId = r.Id,
                            RowNumber = r.RowNumber,
                            Tiers = r.Tiers
                                .OrderBy(t => t.TierNumber)
                                .Select(t => new TierLayoutDto
                                {
                                    TierId = t.Id,
                                    TierNumber = t.TierNumber,
                                    IsOccupied = t.IsOccupied,
                                    ContainerId = t.ContainerId,
                                    ContainerNumber = t.Container?.ContainerNumber
                                }).ToList()
                        }).ToList()
                }).ToList()
        };
        return Success(layout);
    }

    public async Task<ApiResponse<bool>> GenerateLayoutAsync(int blockId, CancellationToken cancellationToken = default)
    {
        var block = await _uow.Blocks.GetBlockWithLayoutAsync(blockId, cancellationToken);
        if (block == null)
            return Failure<bool>("Không tìm thấy block");
        if (block.BlockType == BlockType.Virtual)
            return Failure<bool>("Block ảo không có layout Bay/Row/Tier");

        var ok = await GenerateLayoutInternalAsync(block, cancellationToken);
        return ok ? Success(true, "Sinh layout thành công") : Failure<bool>("Sinh layout thất bại");
    }

    /// <summary>
    /// Tự động sinh Bay/Row/Tier theo MaxBays/MaxRows/MaxTiers.
    /// Áp dụng Factory Pattern: bay chẵn -> 40ft, bay lẻ -> 20ft.
    /// </summary>
    private async Task<bool> GenerateLayoutInternalAsync(Block block, CancellationToken cancellationToken)
    {
        if (!block.MaxBays.HasValue || !block.MaxRows.HasValue || !block.MaxTiers.HasValue)
            return false;

        // Sử dụng DbContext trực tiếp thông qua UnitOfWork để truy cập các DbSet
        var context = ((UnitOfWork)_uow).Context;

        for (int bayNum = 1; bayNum <= block.MaxBays.Value; bayNum++)
        {
            // Bay lẻ -> 20ft, bay chẵn -> 40ft (Factory Pattern)
            var baySize = bayNum % 2 == 1 ? ContainerSize.Size20 : ContainerSize.Size40;

            var existingBay = await context.Bays.FirstOrDefaultAsync(b => b.BlockId == block.Id && b.BayNumber == bayNum);
            if (existingBay != null) continue;

            var bay = new Bay
            {
                BlockId = block.Id,
                BayNumber = bayNum,
                MaxRows = block.MaxRows.Value,
                MaxTiers = block.MaxTiers.Value,
                ContainerSize = baySize
            };
            await context.Bays.AddAsync(bay, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            for (int rowNum = 1; rowNum <= block.MaxRows.Value; rowNum++)
            {
                var row = new Row
                {
                    BayId = bay.Id,
                    RowNumber = rowNum,
                    MaxTiers = block.MaxTiers.Value
                };
                await context.Rows.AddAsync(row, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                for (int tierNum = 1; tierNum <= block.MaxTiers.Value; tierNum++)
                {
                    var tier = new Tier
                    {
                        RowId = row.Id,
                        TierNumber = tierNum,
                        IsOccupied = false
                    };
                    await context.Tiers.AddAsync(tier, cancellationToken);
                }
            }
            await context.SaveChangesAsync(cancellationToken);
        }
        return true;
    }
}
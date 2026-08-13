using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;

namespace DEPOTCONTAINER.Models.DTOs;

/// <summary>
/// DTO trả về thông tin Block và cấu trúc Bay/Row/Tier bên trong.
/// </summary>
public class BlockDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    /// <summary>Alias cho Code - dùng trong BlockLayoutDto</summary>
    public string BlockCode => Code;
    public string Name { get; set; } = string.Empty;
    public BlockType BlockType { get; set; }
    public string BlockTypeName => BlockType.ToString();
    public int? MaxBays { get; set; }
    public int? MaxRows { get; set; }
    public int? MaxTiers { get; set; }
    public ContainerSize? MaxContainerSize { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int TotalBays { get; set; }
    public int ContainerCount { get; set; }

    /// <summary>Danh sách các bay trong block (dùng cho Layout)</summary>
    public List<BayLayoutDto> Bays { get; set; } = new();

    public static BlockDto FromEntity(Block entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        BlockType = entity.BlockType,
        MaxBays = entity.MaxBays,
        MaxRows = entity.MaxRows,
        MaxTiers = entity.MaxTiers,
        MaxContainerSize = entity.MaxContainerSize,
        Description = entity.Description,
        IsActive = entity.IsActive,
        TotalBays = entity.Bays?.Count ?? 0,
        ContainerCount = entity.Containers?.Count ?? 0
    };
}

/// <summary>
/// DTO tạo mới Block.
/// </summary>
public class CreateBlockDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public BlockType BlockType { get; set; } = BlockType.Physical;
    public int? MaxBays { get; set; }
    public int? MaxRows { get; set; }
    public int? MaxTiers { get; set; }
    public ContainerSize? MaxContainerSize { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO hiển thị vị trí trong Block (Bay -> Row -> Tier).
/// </summary>
public class BlockLayoutDto
{
    public int BlockId { get; set; }
    public string BlockCode { get; set; } = string.Empty;
    public List<BayLayoutDto> Bays { get; set; } = new();
}

/// <summary>
/// Một bay trong layout.
/// </summary>
public class BayLayoutDto
{
    public int BayId { get; set; }
    public int BayNumber { get; set; }
    public ContainerSize ContainerSize { get; set; }
    public List<RowLayoutDto> Rows { get; set; } = new();
}

/// <summary>
/// Một row trong layout, kèm các tier.
/// </summary>
public class RowLayoutDto
{
    public int RowId { get; set; }
    public int RowNumber { get; set; }
    public List<TierLayoutDto> Tiers { get; set; } = new();
}

/// <summary>
/// Một tier với thông tin container (nếu có).
/// </summary>
public class TierLayoutDto
{
    public int TierId { get; set; }
    public int TierNumber { get; set; }
    public bool IsOccupied { get; set; }
    public int? ContainerId { get; set; }
    public string? ContainerNumber { get; set; }
}
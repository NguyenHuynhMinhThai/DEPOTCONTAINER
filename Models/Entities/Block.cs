using DEPOTCONTAINER.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Block trong bãi container.
/// Có 2 loại: Physical (quản lý theo Bay/Row/Tier) và Virtual (block ảo).
/// </summary>
public class Block : BaseEntity
{
    /// <summary>Mã block (duy nhất trong hệ thống)</summary>
    [Required]
    [MaxLength(20)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên hiển thị của block</summary>
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Loại block: Vật lý (có bay/row/tier) hoặc ảo</summary>
    [Column("block_type")]
    public BlockType BlockType { get; set; } = BlockType.Physical;

    /// <summary>Số bay tối đa (chỉ áp dụng cho Physical block)</summary>
    [Column("max_bays")]
    public int? MaxBays { get; set; }

    /// <summary>Số row tối đa trong một bay</summary>
    [Column("max_rows")]
    public int? MaxRows { get; set; }

    /// <summary>Số tier tối đa (số tầng xếp chồng tối đa)</summary>
    [Column("max_tiers")]
    public int? MaxTiers { get; set; }

    /// <summary>Kích thước container tối đa của block (20/40/45 feet)</summary>
    [Column("max_container_size")]
    public ContainerSize? MaxContainerSize { get; set; }

    /// <summary>Mô tả / ghi chú</summary>
    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>Trạng thái hoạt động</summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Bay> Bays { get; set; } = new List<Bay>();
    public virtual ICollection<Container> Containers { get; set; } = new List<Container>();
}
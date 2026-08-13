using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Tier trong một Row.
/// Tier là tầng container xếp chồng lên nhau theo chiều thẳng đứng trong mỗi "row".
/// Mỗi tier là một vị trí riêng biệt có thể chứa 0 hoặc 1 container.
/// </summary>
public class Tier : BaseEntity
{
    /// <summary>Khóa ngoại đến Row</summary>
    [Column("row_id")]
    public int RowId { get; set; }

    /// <summary>Số thứ tự của tier (bắt đầu từ 1)</summary>
    [Required]
    [Column("tier_number")]
    public int TierNumber { get; set; }

    /// <summary>Đã có container đặt vào vị trí này chưa</summary>
    [Column("is_occupied")]
    public bool IsOccupied { get; set; } = false;

    /// <summary>Id container hiện đang ở vị trí này (nullable)</summary>
    [Column("container_id")]
    public int? ContainerId { get; set; }

    // Navigation properties
    public virtual Row? Row { get; set; }
    public virtual Container? Container { get; set; }
}
using DEPOTCONTAINER.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Vòng đời của container trong bãi (vào/ra/di chuyển nội bộ).
/// Mỗi lần container được vận chuyển vào hoặc ra khỏi depot sẽ tạo 1 bản ghi.
/// </summary>
public class ContainerMovement : BaseEntity
{
    /// <summary>Khóa ngoại đến Container</summary>
    [Column("container_id")]
    public int ContainerId { get; set; }

    /// <summary>Loại di chuyển: In (vào), Out (ra), Internal (nội bộ)</summary>
    [Column("movement_type")]
    public MovementType MovementType { get; set; }

    /// <summary>Thời điểm thực hiện</summary>
    [Column("movement_date")]
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;

    /// <summary>Phương tiện vận chuyển (số xe, số tàu, số xe nâng...)</summary>
    [Required]
    [MaxLength(100)]
    [Column("vehicle")]
    public string Vehicle { get; set; } = string.Empty;

    /// <summary>Loại phương tiện (xe tải, tàu biển, xe nâng...)</summary>
    [MaxLength(50)]
    [Column("vehicle_type")]
    public string? VehicleType { get; set; }

    /// <summary>Số seal (niêm phong)</summary>
    [MaxLength(50)]
    [Column("seal_number")]
    public string? SealNumber { get; set; }

    /// <summary>Vị trí ban đầu (cho di chuyển nội bộ)</summary>
    [MaxLength(100)]
    [Column("from_location")]
    public string? FromLocation { get; set; }

    /// <summary>Vị trí đích (cho di chuyển nội bộ)</summary>
    [MaxLength(100)]
    [Column("to_location")]
    public string? ToLocation { get; set; }

    /// <summary>Khóa ngoại đến Block sau khi di chuyển</summary>
    [Column("to_block_id")]
    public int? ToBlockId { get; set; }

    /// <summary>Tên tài xế / người vận chuyển</summary>
    [MaxLength(100)]
    [Column("driver_name")]
    public string? DriverName { get; set; }

    /// <summary>Số giấy tờ tài xế / thuyền trưởng</summary>
    [MaxLength(50)]
    [Column("driver_id")]
    public string? DriverId { get; set; }

    /// <summary>Khóa ngoại đến ReleaseOrder (nếu là lệnh giao)</summary>
    [Column("release_order_id")]
    public int? ReleaseOrderId { get; set; }

    /// <summary>Ghi chú</summary>
    [MaxLength(500)]
    [Column("note")]
    public string? Note { get; set; }

    // Navigation properties
    public virtual Container? Container { get; set; }
    public virtual Block? ToBlock { get; set; }
    public virtual ReleaseOrder? ReleaseOrder { get; set; }
}
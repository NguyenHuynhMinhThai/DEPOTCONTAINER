using DEPOTCONTAINER.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Lệnh giao container từ hãng khai thác (Line Operator).
/// Depot KHÔNG ĐƯỢC tự ý giao container ra ngoài, phải có lệnh này.
/// </summary>
public class ReleaseOrder : BaseEntity
{
    /// <summary>Số lệnh giao (duy nhất)</summary>
    [Required]
    [MaxLength(50)]
    [Column("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>Khóa ngoại đến Line Operator (hãng ra lệnh)</summary>
    [Column("line_operator_id")]
    public int LineOperatorId { get; set; }

    /// <summary>Khóa ngoại đến Customer (khách hàng nhận)</summary>
    [Column("customer_id")]
    public int CustomerId { get; set; }

    /// <summary>Hạn lệnh - ngày cuối cùng được phép giao container</summary>
    [Column("valid_until")]
    public DateTime ValidUntil { get; set; }

    /// <summary>Chuyến tàu xuất container ra khỏi Việt Nam</summary>
    [MaxLength(100)]
    [Column("export_vessel")]
    public string? ExportVessel { get; set; }

    /// <summary>Ngày tàu dự kiến xuất</summary>
    [Column("export_date")]
    public DateTime? ExportDate { get; set; }

    /// <summary>Trạng thái lệnh</summary>
    [Column("status")]
    public ReleaseOrderStatus Status { get; set; } = ReleaseOrderStatus.New;

    /// <summary>Mô tả chi tiết / ghi chú</summary>
    [MaxLength(1000)]
    [Column("description")]
    public string? Description { get; set; }

    // Navigation properties
    public virtual LineOperator? LineOperator { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual ICollection<ReleaseOrderDetail> Details { get; set; } = new List<ReleaseOrderDetail>();
    public virtual ICollection<ContainerMovement> Movements { get; set; } = new List<ContainerMovement>();
}
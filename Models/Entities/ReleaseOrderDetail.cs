using DEPOTCONTAINER.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Chi tiết lệnh giao: cho biết cần giao bao nhiêu container mỗi loại.
/// Ví dụ: 10 container 20ft Dry, 5 container 40ft Reefer...
/// </summary>
public class ReleaseOrderDetail : BaseEntity
{
    /// <summary>Khóa ngoại đến ReleaseOrder</summary>
    [Column("release_order_id")]
    public int ReleaseOrderId { get; set; }

    /// <summary>Kích thước container (20/40/45 feet)</summary>
    [Column("container_size")]
    public ContainerSize ContainerSize { get; set; }

    /// <summary>Loại container (Dry/Reefer/OpenTop...)</summary>
    [Column("container_type")]
    public ContainerType ContainerType { get; set; }

    /// <summary>Số lượng container cần giao</summary>
    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>Số lượng đã giao thực tế</summary>
    [Column("delivered_quantity")]
    public int DeliveredQuantity { get; set; } = 0;

    /// <summary>Ghi chú</summary>
    [MaxLength(500)]
    [Column("note")]
    public string? Note { get; set; }

    // Navigation property
    public virtual ReleaseOrder? ReleaseOrder { get; set; }
}
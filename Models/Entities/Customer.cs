using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Khách hàng nhận container (theo lệnh giao).
/// Lưu trữ MST và tên khách hàng.
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>Mã số thuế (MST) - định danh duy nhất khách hàng</summary>
    [Required]
    [MaxLength(20)]
    [Column("tax_code")]
    public string TaxCode { get; set; } = string.Empty;

    /// <summary>Tên khách hàng / doanh nghiệp</summary>
    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Địa chỉ</summary>
    [MaxLength(500)]
    [Column("address")]
    public string? Address { get; set; }

    /// <summary>Số điện thoại</summary>
    [MaxLength(20)]
    [Column("phone")]
    public string? Phone { get; set; }

    /// <summary>Người liên hệ</summary>
    [MaxLength(100)]
    [Column("contact_person")]
    public string? ContactPerson { get; set; }

    /// <summary>Ghi chú</summary>
    [MaxLength(1000)]
    [Column("note")]
    public string? Note { get; set; }

    // Navigation property
    public virtual ICollection<ReleaseOrder> ReleaseOrders { get; set; } = new List<ReleaseOrder>();
}
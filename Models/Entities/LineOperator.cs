using DEPOTCONTAINER.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Hãng khai thác container (Line Operator).
/// Ví dụ: CMA CGM, MSC, HMM, Maersk...
/// Mã 3 chữ cái đầu của Container Number chính là Owner Code của hãng.
/// </summary>
public class LineOperator : BaseEntity
{
    /// <summary>Mã chủ sở hữu container (3 chữ cái đầu của Container Number)</summary>
    [Required]
    [MaxLength(4)]
    [Column("owner_code")]
    public string OwnerCode { get; set; } = string.Empty;

    /// <summary>Tên đầy đủ của hãng khai thác</summary>
    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Mã số thuế / mã doanh nghiệp</summary>
    [MaxLength(50)]
    [Column("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>Địa chỉ liên hệ</summary>
    [MaxLength(500)]
    [Column("address")]
    public string? Address { get; set; }

    /// <summary>Số điện thoại</summary>
    [MaxLength(20)]
    [Column("phone")]
    public string? Phone { get; set; }

    /// <summary>Email liên hệ</summary>
    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    /// <summary>Ghi chú</summary>
    [MaxLength(1000)]
    [Column("note")]
    public string? Note { get; set; }

    // Navigation property
    public virtual ICollection<Container> Containers { get; set; } = new List<Container>();
    public virtual ICollection<ReleaseOrder> ReleaseOrders { get; set; } = new List<ReleaseOrder>();
}
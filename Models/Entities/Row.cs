using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Row trong một Bay.
/// Row là hàng container theo chiều rộng (hướng từ trước ra sau) trong một "bay".
/// </summary>
public class Row : BaseEntity
{
    /// <summary>Khóa ngoại đến Bay</summary>
    [Column("bay_id")]
    public int BayId { get; set; }

    /// <summary>Số thứ tự của row trong bay</summary>
    [Required]
    [Column("row_number")]
    public int RowNumber { get; set; }

    /// <summary>Số tier tối đa trong row này</summary>
    [Column("max_tiers")]
    public int MaxTiers { get; set; } = 5;

    /// <summary>Ghi chú</summary>
    [MaxLength(200)]
    [Column("note")]
    public string? Note { get; set; }

    // Navigation properties
    public virtual Bay? Bay { get; set; }
    public virtual ICollection<Tier> Tiers { get; set; } = new List<Tier>();
}
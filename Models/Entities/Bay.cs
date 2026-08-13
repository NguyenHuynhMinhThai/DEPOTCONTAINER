using DEPOTCONTAINER.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Bay trong một Block.
/// Bay lẻ (1, 3, 5...) chứa container 20ft.
/// Bay chẵn (2, 4, 6...) chứa container 40ft (2 bay lẻ = 1 bay chẵn).
/// </summary>
public class Bay : BaseEntity
{
    /// <summary>Khóa ngoại đến Block</summary>
    [Column("block_id")]
    public int BlockId { get; set; }

    /// <summary>Số thứ tự của bay trong block</summary>
    [Required]
    [Column("bay_number")]
    public int BayNumber { get; set; }

    /// <summary>Số row tối đa trong bay này</summary>
    [Column("max_rows")]
    public int MaxRows { get; set; } = 10;

    /// <summary>Số tier tối đa trong bay này</summary>
    [Column("max_tiers")]
    public int MaxTiers { get; set; } = 5;

    /// <summary>Kích thước container mà bay này chứa</summary>
    [Column("container_size")]
    public ContainerSize ContainerSize { get; set; } = ContainerSize.Size20;

    /// <summary>Ghi chú</summary>
    [MaxLength(200)]
    [Column("note")]
    public string? Note { get; set; }

    // Navigation properties
    public virtual Block? Block { get; set; }
    public virtual ICollection<Row> Rows { get; set; } = new List<Row>();
}
using DEPOTCONTAINER.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Container trong hệ thống depot.
/// Số container theo cấu trúc ISO 6346:
/// - Owner Code (3 chữ cái): Mã chủ sở hữu
/// - Type Code (1 chữ cái): Mã loại (U, R, S, F...)
/// - Serial Number (6 chữ số): Số dãy
/// - Check Digit (1 chữ số hoặc X): Mã kiểm tra theo Modulo 11
/// Tổng cộng: 4 chữ cái + 7 chữ số = 11 ký tự (ví dụ: CMAU1234567)
/// </summary>
public class Container : BaseEntity
{
    /// <summary>Số container đầy đủ 11 ký tự (Owner + Type + Serial + Check)</summary>
    [Required]
    [MaxLength(11)]
    [Column("container_number")]
    public string ContainerNumber { get; set; } = string.Empty;

    /// <summary>Loại container theo ISO (U=Dry, R=Reefer...)</summary>
    [Column("container_type")]
    public ContainerType ContainerType { get; set; }

    /// <summary>Mã ISO đầy đủ 4 ký tự (ví dụ: 22G1, 45R1)</summary>
    [Required]
    [MaxLength(10)]
    [Column("iso_code")]
    public string IsoCode { get; set; } = string.Empty;

    /// <summary>Kích thước container (20/40/45 feet)</summary>
    [Column("size")]
    public ContainerSize Size { get; set; }

    /// <summary>Trọng lượng tối đa cho phép (kg) - Maximum Weight</summary>
    [Column("max_weight")]
    public decimal MaxWeight { get; set; }

    /// <summary>Trọng lượng vỏ container (kg) - Tare Weight</summary>
    [Column("tare_weight")]
    public decimal TareWeight { get; set; }

    /// <summary>Ngày sản xuất</summary>
    [Column("manufacture_date")]
    public DateTime? ManufactureDate { get; set; }

    /// <summary>Khóa ngoại đến Line Operator (chủ sở hữu)</summary>
    [Column("line_operator_id")]
    public int? LineOperatorId { get; set; }

    /// <summary>Khóa ngoại đến Block hiện tại (null nếu chưa vào bãi)</summary>
    [Column("current_block_id")]
    public int? CurrentBlockId { get; set; }

    /// <summary>Khóa ngoại đến Bay hiện tại</summary>
    [Column("current_bay_id")]
    public int? CurrentBayId { get; set; }

    /// <summary>Khóa ngoại đến Row hiện tại</summary>
    [Column("current_row_id")]
    public int? CurrentRowId { get; set; }

    /// <summary>Khóa ngoại đến Tier hiện tại</summary>
    [Column("current_tier_id")]
    public int? CurrentTierId { get; set; }

    /// <summary>Tình trạng container (Normal / Damaged)</summary>
    [Column("condition")]
    public ContainerCondition Condition { get; set; } = ContainerCondition.Normal;

    /// <summary>Phân loại container (A/B/C)</summary>
    [Column("category")]
    public ContainerCategory Category { get; set; } = ContainerCategory.CategoryA;

    /// <summary>Mô tả hư hỏng (nếu có)</summary>
    [MaxLength(1000)]
    [Column("damage_description")]
    public string? DamageDescription { get; set; }

    /// <summary>Trạng thái đang ở trong bãi</summary>
    [Column("is_in_yard")]
    public bool IsInYard { get; set; } = false;

    // Navigation properties
    public virtual LineOperator? LineOperator { get; set; }
    public virtual Block? CurrentBlock { get; set; }
    public virtual Bay? CurrentBay { get; set; }
    public virtual Row? CurrentRow { get; set; }
    public virtual Tier? CurrentTier { get; set; }
    public virtual ICollection<ContainerMovement> Movements { get; set; } = new List<ContainerMovement>();
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPOTCONTAINER.Models.Entities;

/// <summary>
/// Lớp nền cho tất cả entity trong hệ thống.
/// Cung cấp các trường audit (Created/Updated/IsDeleted) cho tất cả bảng.
/// Đây là minh họa cho nguyên lý DRY và Generic Repository Pattern.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Khóa chính - tự tăng</summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>Ngày tạo bản ghi (UTC)</summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Ngày cập nhật gần nhất (UTC)</summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Cờ xóa mềm (soft delete)</summary>
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
}
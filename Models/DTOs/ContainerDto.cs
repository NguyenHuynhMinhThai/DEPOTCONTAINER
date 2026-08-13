using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;

namespace DEPOTCONTAINER.Models.DTOs;

/// <summary>
/// DTO dùng cho việc trả về thông tin container cho client (API/Razor Pages).
/// Tránh việc trả về trực tiếp entity (Entity Framework tracking) ra ngoài.
/// </summary>
public class ContainerDto
{
    public int Id { get; set; }
    public string ContainerNumber { get; set; } = string.Empty;
    public ContainerType ContainerType { get; set; }
    public string ContainerTypeName => ContainerType.ToString();
    public string IsoCode { get; set; } = string.Empty;
    public ContainerSize Size { get; set; }
    public int SizeInFeet => (int)Size;
    public decimal MaxWeight { get; set; }
    public decimal TareWeight { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public int? LineOperatorId { get; set; }
    public string? LineOperatorName { get; set; }
    public int? CurrentBlockId { get; set; }
    public string? CurrentBlockCode { get; set; }
    public int? CurrentBayId { get; set; }
    public int? CurrentBayNumber { get; set; }
    public int? CurrentRowId { get; set; }
    public int? CurrentRowNumber { get; set; }
    public int? CurrentTierId { get; set; }
    public int? CurrentTierNumber { get; set; }
    public ContainerCondition Condition { get; set; }
    public string ConditionName => Condition.ToString();
    public ContainerCategory Category { get; set; }
    public string CategoryName => Category.ToString();
    public string? DamageDescription { get; set; }
    public bool IsInYard { get; set; }
    public string Location => BuildLocation();

    private string BuildLocation()
    {
        if (!CurrentBlockId.HasValue) return "(chưa vào bãi)";
        var parts = new List<string> { CurrentBlockCode ?? "?" };
        if (CurrentBayNumber.HasValue) parts.Add($"Bay {CurrentBayNumber}");
        if (CurrentRowNumber.HasValue) parts.Add($"Row {CurrentRowNumber}");
        if (CurrentTierNumber.HasValue) parts.Add($"Tier {CurrentTierNumber}");
        return string.Join(" - ", parts);
    }

    public static ContainerDto FromEntity(Container entity) => new()
    {
        Id = entity.Id,
        ContainerNumber = entity.ContainerNumber,
        ContainerType = entity.ContainerType,
        IsoCode = entity.IsoCode,
        Size = entity.Size,
        MaxWeight = entity.MaxWeight,
        TareWeight = entity.TareWeight,
        ManufactureDate = entity.ManufactureDate,
        LineOperatorId = entity.LineOperatorId,
        LineOperatorName = entity.LineOperator?.Name,
        CurrentBlockId = entity.CurrentBlockId,
        CurrentBlockCode = entity.CurrentBlock?.Code,
        CurrentBayId = entity.CurrentBayId,
        CurrentBayNumber = entity.CurrentBay?.BayNumber,
        CurrentRowId = entity.CurrentRowId,
        CurrentRowNumber = entity.CurrentRow?.RowNumber,
        CurrentTierId = entity.CurrentTierId,
        CurrentTierNumber = entity.CurrentTier?.TierNumber,
        Condition = entity.Condition,
        Category = entity.Category,
        DamageDescription = entity.DamageDescription,
        IsInYard = entity.IsInYard
    };
}

/// <summary>
/// DTO cho việc tạo mới container.
/// </summary>
public class CreateContainerDto
{
    public string ContainerNumber { get; set; } = string.Empty;
    public ContainerType ContainerType { get; set; }
    public string IsoCode { get; set; } = string.Empty;
    public ContainerSize Size { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal TareWeight { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public int? LineOperatorId { get; set; }
    public ContainerCondition Condition { get; set; } = ContainerCondition.Normal;
    public ContainerCategory Category { get; set; } = ContainerCategory.CategoryA;
    public string? DamageDescription { get; set; }
}

/// <summary>
/// DTO cho việc cập nhật container.
/// </summary>
public class UpdateContainerDto
{
    public int Id { get; set; }
    public ContainerType ContainerType { get; set; }
    public string IsoCode { get; set; } = string.Empty;
    public ContainerSize Size { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal TareWeight { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public int? LineOperatorId { get; set; }
    public ContainerCondition Condition { get; set; }
    public ContainerCategory Category { get; set; }
    public string? DamageDescription { get; set; }
}
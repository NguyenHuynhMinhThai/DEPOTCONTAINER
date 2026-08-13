using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;

namespace DEPOTCONTAINER.Models.DTOs;

/// <summary>
/// DTO trả về thông tin Line Operator.
/// </summary>
public class LineOperatorDto
{
    public int Id { get; set; }
    public string OwnerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Note { get; set; }

    public static LineOperatorDto FromEntity(LineOperator entity) => new()
    {
        Id = entity.Id,
        OwnerCode = entity.OwnerCode,
        Name = entity.Name,
        TaxCode = entity.TaxCode,
        Address = entity.Address,
        Phone = entity.Phone,
        Email = entity.Email,
        Note = entity.Note
    };
}

/// <summary>
/// DTO trả về thông tin Customer.
/// </summary>
public class CustomerDto
{
    public int Id { get; set; }
    public string TaxCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactPerson { get; set; }
    public string? Note { get; set; }

    public static CustomerDto FromEntity(Customer entity) => new()
    {
        Id = entity.Id,
        TaxCode = entity.TaxCode,
        Name = entity.Name,
        Address = entity.Address,
        Phone = entity.Phone,
        ContactPerson = entity.ContactPerson,
        Note = entity.Note
    };
}

/// <summary>
/// DTO cho container movement (vòng đời vào/ra bãi).
/// </summary>
public class ContainerMovementDto
{
    public int Id { get; set; }
    public int ContainerId { get; set; }
    public string? ContainerNumber { get; set; }
    public MovementType MovementType { get; set; }
    public string MovementTypeName => MovementType.ToString();
    public DateTime MovementDate { get; set; }
    public string Vehicle { get; set; } = string.Empty;
    public string? VehicleType { get; set; }
    public string? SealNumber { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public int? ToBlockId { get; set; }
    public string? ToBlockCode { get; set; }
    public string? DriverName { get; set; }
    public string? DriverId { get; set; }
    public int? ReleaseOrderId { get; set; }
    public string? ReleaseOrderNumber { get; set; }
    public string? Note { get; set; }

    public static ContainerMovementDto FromEntity(ContainerMovement entity) => new()
    {
        Id = entity.Id,
        ContainerId = entity.ContainerId,
        ContainerNumber = entity.Container?.ContainerNumber,
        MovementType = entity.MovementType,
        MovementDate = entity.MovementDate,
        Vehicle = entity.Vehicle,
        VehicleType = entity.VehicleType,
        SealNumber = entity.SealNumber,
        FromLocation = entity.FromLocation,
        ToLocation = entity.ToLocation,
        ToBlockId = entity.ToBlockId,
        ToBlockCode = entity.ToBlock?.Code,
        DriverName = entity.DriverName,
        DriverId = entity.DriverId,
        ReleaseOrderId = entity.ReleaseOrderId,
        ReleaseOrderNumber = entity.ReleaseOrder?.OrderNumber,
        Note = entity.Note
    };
}

/// <summary>
/// DTO cho việc tạo mới movement (container vào/ra bãi).
/// </summary>
public class CreateMovementDto
{
    public int ContainerId { get; set; }
    public MovementType MovementType { get; set; }
    public string Vehicle { get; set; } = string.Empty;
    public string? VehicleType { get; set; }
    public string? SealNumber { get; set; }
    public int? ToBlockId { get; set; }
    public string? DriverName { get; set; }
    public string? DriverId { get; set; }
    public int? ReleaseOrderId { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// DTO cho ReleaseOrder.
/// </summary>
public class ReleaseOrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int LineOperatorId { get; set; }
    public string? LineOperatorName { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxCode { get; set; }
    public DateTime ValidUntil { get; set; }
    public string? ExportVessel { get; set; }
    public DateTime? ExportDate { get; set; }
    public ReleaseOrderStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? Description { get; set; }
    public List<ReleaseOrderDetailDto> Details { get; set; } = new();

    public static ReleaseOrderDto FromEntity(ReleaseOrder entity) => new()
    {
        Id = entity.Id,
        OrderNumber = entity.OrderNumber,
        LineOperatorId = entity.LineOperatorId,
        LineOperatorName = entity.LineOperator?.Name,
        CustomerId = entity.CustomerId,
        CustomerName = entity.Customer?.Name,
        CustomerTaxCode = entity.Customer?.TaxCode,
        ValidUntil = entity.ValidUntil,
        ExportVessel = entity.ExportVessel,
        ExportDate = entity.ExDate(),
        Status = entity.Status,
        Description = entity.Description,
        Details = entity.Details?.Select(d => ReleaseOrderDetailDto.FromEntity(d)).ToList() ?? new()
    };
}

/// <summary>
/// Helper extension trong DTO.
/// </summary>
internal static class ReleaseOrderExtensions
{
    public static DateTime? ExDate(this ReleaseOrder entity) => entity.ExportDate;
}

/// <summary>
/// DTO cho chi tiết ReleaseOrder.
/// </summary>
public class ReleaseOrderDetailDto
{
    public int Id { get; set; }
    public ContainerSize ContainerSize { get; set; }
    public int SizeInFeet => (int)ContainerSize;
    public ContainerType ContainerType { get; set; }
    public string ContainerTypeName => ContainerType.ToString();
    public int Quantity { get; set; }
    public int DeliveredQuantity { get; set; }
    public int RemainingQuantity => Quantity - DeliveredQuantity;
    public string? Note { get; set; }

    public static ReleaseOrderDetailDto FromEntity(ReleaseOrderDetail entity) => new()
    {
        Id = entity.Id,
        ContainerSize = entity.ContainerSize,
        ContainerType = entity.ContainerType,
        Quantity = entity.Quantity,
        DeliveredQuantity = entity.DeliveredQuantity,
        Note = entity.Note
    };
}

/// <summary>
/// DTO tạo ReleaseOrder.
/// </summary>
public class CreateReleaseOrderDto
{
    public string OrderNumber { get; set; } = string.Empty;
    public int LineOperatorId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ValidUntil { get; set; }
    public string? ExportVessel { get; set; }
    public DateTime? ExportDate { get; set; }
    public string? Description { get; set; }
    public List<CreateReleaseOrderDetailDto> Details { get; set; } = new();
}

/// <summary>
/// DTO tạo chi tiết ReleaseOrder.
/// </summary>
public class CreateReleaseOrderDetailDto
{
    public ContainerSize ContainerSize { get; set; }
    public ContainerType ContainerType { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}
namespace DEPOTCONTAINER.Models.Enums;

/// <summary>
/// Loại container theo tiêu chuẩn ISO 6346.
/// Mỗi mã tương ứng với một chữ cái trong phần "Type Code" của số container.
/// </summary>
public enum ContainerType
{
    /// <summary>Container khô (Dry Container)</summary>
    Dry = 'U',

    /// <summary>Container lạnh (Reefer Container)</summary>
    Reefer = 'R',

    /// <summary>Container mở nắp (Open Top Container)</summary>
    OpenTop = 'S',

    /// <summary>Container phẳng (Flat Rack Container)</summary>
    FlatRack = 'F',

    /// <summary>Container chứa hàng nguy hiểm (Bunker)</summary>
    Bunker = 'B',

    /// <summary>Container thông gió (Ventilated)</summary>
    Ventilated = 'V',

    /// <summary>Container chuyên dụng (Specialized)</summary>
    Specialized = 'Z'
}

/// <summary>
/// Kích thước container tính theo feet.
/// Bay lẻ chứa container 20ft, bay chẵn chứa container 40ft.
/// </summary>
public enum ContainerSize
{
    /// <summary>Container 20 feet</summary>
    Size20 = 20,

    /// <summary>Container 40 feet</summary>
    Size40 = 40,

    /// <summary>Container 45 feet</summary>
    Size45 = 45
}

/// <summary>
/// Tình trạng container trong bãi.
/// </summary>
public enum ContainerCondition
{
    /// <summary>Bình thường, không hư hỏng</summary>
    Normal = 0,

    /// <summary>Có hư hỏng nhẹ (móp, trầy)</summary>
    SlightlyDamaged = 1,

    /// <summary>Hư hỏng nặng (cong, vênh, thủng)</summary>
    SeverelyDamaged = 2
}

/// <summary>
/// Phân loại container theo hãng khai thác.
/// Thường dùng để phân nhóm A (mới), B (cũ), C (hỏng)...
/// </summary>
public enum ContainerCategory
{
    /// <summary>Loại A - container còn tốt, sử dụng được ngay</summary>
    CategoryA = 0,

    /// <summary>Loại B - container đã qua sử dụng</summary>
    CategoryB = 1,

    /// <summary>Loại C - container cần sửa chữa</summary>
    CategoryC = 2
}

/// <summary>
/// Loại block trong bãi.
/// </summary>
public enum BlockType
{
    /// <summary>Block vật lý, quản lý theo Bay/Row/Tier</summary>
    Physical = 0,

    /// <summary>Block ảo, dùng cho điều hành đặc biệt</summary>
    Virtual = 1
}

/// <summary>
/// Loại chuyển động của container (vào/ra bãi).
/// </summary>
public enum MovementType
{
    /// <summary>Container vào bãi (In)</summary>
    In = 0,

    /// <summary>Container ra khỏi bãi (Out)</summary>
    Out = 1,

    /// <summary>Di chuyển nội bộ trong bãi</summary>
    Internal = 2
}

/// <summary>
/// Trạng thái lệnh giao container.
/// </summary>
public enum ReleaseOrderStatus
{
    /// <summary>Mới tạo, chưa thực hiện</summary>
    New = 0,

    /// <summary>Đang thực hiện giao container</summary>
    InProgress = 1,

    /// <summary>Đã hoàn thành</summary>
    Completed = 2,

    /// <summary>Đã hủy</summary>
    Cancelled = 3,

    /// <summary>Đã hết hạn</summary>
    Expired = 4
}
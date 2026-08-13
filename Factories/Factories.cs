using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;

namespace DEPOTCONTAINER.Factories;

/// <summary>
/// Factory Pattern: tạo đối tượng Bay với ContainerSize phù hợp.
/// Theo đề bài: Bay lẻ chứa container 20ft, bay chẵn chứa container 40ft.
/// (2 bay lẻ tương đương 1 bay chẵn).
/// </summary>
public static class BayFactory
{
    /// <summary>
    /// Xác định kích thước container mà 1 bay được phép chứa dựa vào số thứ tự bay.
    /// </summary>
    /// <param name="bayNumber">Số thứ tự bay (bắt đầu từ 1)</param>
    /// <returns>ContainerSize phù hợp</returns>
    public static ContainerSize DetermineContainerSize(int bayNumber)
    {
        if (bayNumber <= 0)
            throw new ArgumentException("Số bay phải lớn hơn 0.", nameof(bayNumber));

        // Bay lẻ -> 20ft, Bay chẵn -> 40ft
        return bayNumber % 2 == 1 ? ContainerSize.Size20 : ContainerSize.Size40;
    }

    /// <summary>
    /// Tạo mới một Bay đã cấu hình sẵn dựa vào block và số thứ tự.
    /// </summary>
    public static Bay CreateBay(Block block, int bayNumber, int maxRows, int maxTiers)
    {
        ArgumentNullException.ThrowIfNull(block);

        return new Bay
        {
            BlockId = block.Id,
            BayNumber = bayNumber,
            MaxRows = maxRows,
            MaxTiers = maxTiers,
            ContainerSize = DetermineContainerSize(bayNumber)
        };
    }

    /// <summary>
    /// Tạo hàng loạt Bay trong một block (1..maxBays).
    /// </summary>
    public static List<Bay> CreateBaysForBlock(Block block, int maxBays, int maxRows, int maxTiers)
    {
        ArgumentNullException.ThrowIfNull(block);

        var bays = new List<Bay>();
        for (int i = 1; i <= maxBays; i++)
        {
            bays.Add(CreateBay(block, i, maxRows, maxTiers));
        }
        return bays;
    }
}

/// <summary>
/// Factory Pattern: tạo container từ số container ISO 6346.
/// Tự động phân tích Owner Code, Type Code, Serial Number từ số container.
/// </summary>
public static class ContainerFactory
{
    /// <summary>
    /// Phân tích số container và trả về đối tượng ContainerDto mặc định.
    /// </summary>
    public static Container CreateFromNumber(string containerNumber)
    {
        if (string.IsNullOrWhiteSpace(containerNumber))
            throw new ArgumentException("Số container không được để trống");

        var info = DEPOTCONTAINER.Validators.ContainerNumberValidator.Parse(containerNumber);

        // Ánh xạ Type Code -> ContainerType
        var type = (ContainerType)info.CategoryIdentifier;

        // Ánh xạ Owner Code -> LineOperatorId (cần lookup, tạm thời null)
        return new Container
        {
            ContainerNumber = containerNumber.ToUpper(),
            ContainerType = type,
            IsoCode = string.Empty, // Cần nhập thủ công hoặc lookup từ DB
            Size = type == ContainerType.Reefer ? ContainerSize.Size40 : ContainerSize.Size20,
            Condition = ContainerCondition.Normal,
            Category = ContainerCategory.CategoryA
        };
    }
}

/// <summary>
/// Generic Factory - tạo entity mặc định cho bất kỳ loại nào.
/// </summary>
/// <typeparam name="T">Loại entity (phải có constructor không tham số)</typeparam>
public static class GenericFactory<T> where T : new()
{
    /// <summary>
    /// Tạo 1 instance mới của T.
    /// </summary>
    public static T Create()
    {
        return new T();
    }

    /// <summary>
    /// Tạo nhiều instance.
    /// </summary>
    public static List<T> CreateMany(int count)
    {
        if (count <= 0) throw new ArgumentException("Số lượng phải > 0", nameof(count));
        var list = new List<T>(count);
        for (int i = 0; i < count; i++) list.Add(new T());
        return list;
    }
}
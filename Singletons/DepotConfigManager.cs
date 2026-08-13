using DEPOTCONTAINER.Models.Enums;

namespace DEPOTCONTAINER.Singletons;

/// <summary>
/// Singleton Pattern: lưu trữ cấu hình runtime cho toàn hệ thống.
/// Có thể truy cập từ bất kỳ đâu, đảm bảo chỉ có 1 instance duy nhất.
/// Trong .NET, DI container tự động quản lý Singleton lifetime.
/// Lớp này minh họa cách tự cài đặt Singleton Pattern bằng double-check locking.
/// </summary>
public sealed class DepotConfigManager
{
    private static DepotConfigManager? _instance;
    private static readonly object _lock = new();

    /// <summary>Các tham số cấu hình runtime (có thể thay đổi khi hệ thống chạy)</summary>
    public DepotSettings Settings { get; private set; } = new();

    /// <summary>Số lượng tối đa tier (số tầng xếp chồng tối đa trong 1 row)</summary>
    public int MaxTiersPerRow => Settings.MaxTiersPerRow;

    private DepotConfigManager() { }

    /// <summary>Lấy instance duy nhất (Singleton)</summary>
    public static DepotConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new DepotConfigManager();
                }
            }
            return _instance;
        }
    }

    /// <summary>Cập nhật cấu hình</summary>
    public void UpdateSettings(DepotSettings settings)
    {
        lock (_lock)
        {
            Settings = settings;
        }
    }

    /// <summary>Kiểm tra 1 container size có hợp lệ với bay không</summary>
    public bool IsValidBayContainerSize(int bayNumber, ContainerSize containerSize)
    {
        var baySize = bayNumber % 2 == 1 ? ContainerSize.Size20 : ContainerSize.Size40;
        return baySize == containerSize;
    }
}

/// <summary>
/// Cấu hình hệ thống - có thể load từ database hoặc appsettings.
/// </summary>
public class DepotSettings
{
    public int MaxTiersPerRow { get; set; } = 5;
    public int MaxRowsPerBay { get; set; } = 10;
    public int MaxBaysPerBlock { get; set; } = 20;
    public bool AllowVirtualBlocks { get; set; } = true;
    public int DefaultBlockCode { get; set; } = 1;
}

/// <summary>
/// Singleton service quản lý cache trong bộ nhớ.
/// Minh họa cách dùng Singleton + Generic.
/// </summary>
public sealed class InMemoryCache
{
    private static InMemoryCache? _instance;
    private static readonly object _lock = new();
    private readonly Dictionary<string, object> _cache = new();
    private readonly Dictionary<string, DateTime> _expirations = new();

    private InMemoryCache() { }

    public static InMemoryCache Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock) { _instance ??= new InMemoryCache(); }
            }
            return _instance;
        }
    }

    public void Set<T>(string key, T value, TimeSpan? ttl = null)
    {
        lock (_lock)
        {
            _cache[key] = value!;
            _expirations[key] = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : DateTime.MaxValue;
        }
    }

    public T? Get<T>(string key)
    {
        lock (_lock)
        {
            if (!_cache.TryGetValue(key, out var value)) return default;
            if (_expirations.TryGetValue(key, out var exp) && exp < DateTime.UtcNow)
            {
                _cache.Remove(key);
                _expirations.Remove(key);
                return default;
            }
            return (T)value;
        }
    }

    public void Remove(string key)
    {
        lock (_lock)
        {
            _cache.Remove(key);
            _expirations.Remove(key);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _cache.Clear();
            _expirations.Clear();
        }
    }

    public int Count
    {
        get { lock (_lock) { return _cache.Count; } }
    }
}
namespace DEPOTCONTAINER.Models.DTOs;

/// <summary>
/// Kết quả trả về thống nhất cho tất cả API.
/// Áp dụng Generic Pattern - có thể dùng cho bất kỳ kiểu dữ liệu nào.
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của payload</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string message = "Thành công") => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors ?? new List<string>()
    };
}

/// <summary>
/// DTO phân trang (pagination) - Generic cho mọi danh sách.
/// Áp dụng Generic + LINQ.
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của item</typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10) => new()
    {
        Items = new List<T>(),
        TotalCount = 0,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}

/// <summary>
/// Tham số truy vấn chung cho danh sách có phân trang và sắp xếp.
/// Áp dụng Predicate + Func qua các extension method.
/// </summary>
public class QueryParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    public string? SearchTerm { get; set; }
}
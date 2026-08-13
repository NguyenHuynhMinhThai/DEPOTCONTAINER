using System.Linq.Expressions;
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Entities;

namespace DEPOTCONTAINER.Extensions;

/// <summary>
/// Extension Methods cho IEnumerable và IQueryable.
/// Áp dụng Generic + LINQ + Lambda expression.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Lọc các phần tử theo điều kiện (nếu điều kiện null thì trả về toàn bộ).
    /// </summary>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate,
        bool condition)
    {
        return condition ? source.Where(predicate) : source;
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        Expression<Func<T, bool>> predicate,
        bool condition)
    {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Kiểm tra danh sách có rỗng không (tương tự Any() nhưng tường minh hơn).
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Tìm phần tử đầu tiên thỏa mãn điều kiện, trả về default nếu không có.
    /// </summary>
    public static T? FirstOrDefaultSafe<T>(this IEnumerable<T> source, Func<T, bool> predicate, T? defaultValue = default)
    {
        return source.FirstOrDefault(predicate) ?? defaultValue;
    }

    /// <summary>
    /// Group by và đếm theo key.
    /// </summary>
    public static Dictionary<TKey, int> CountBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) where TKey : notnull
    {
        return source.GroupBy(keySelector).ToDictionary(g => g.Key, g => g.Count());
    }
}

/// <summary>
/// Extension cho BaseEntity - cập nhật timestamp tự động.
/// </summary>
public static class BaseEntityExtensions
{
    public static void Touch<T>(this T entity) where T : BaseEntity
    {
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static bool IsNew<T>(this T entity) where T : BaseEntity
    {
        return entity.Id == 0;
    }
}

/// <summary>
/// Extension cho string - các helper phổ biến.
/// </summary>
public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

    public static string OrEmpty(this string? str) => str ?? string.Empty;

    public static string Truncate(this string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str) || str.Length <= maxLength) return str;
        return str.Substring(0, maxLength) + "...";
    }
}

/// <summary>
/// Extension cho decimal - format tiền tệ/trọng lượng.
/// </summary>
public static class DecimalExtensions
{
    public static string ToWeightString(this decimal weight) => $"{weight:N2} kg";

    public static string ToVietnameseCurrency(this decimal amount) => $"{amount:N0} VND";
}
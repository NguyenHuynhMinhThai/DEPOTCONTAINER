using System.Text.RegularExpressions;

namespace DEPOTCONTAINER.Validators;

/// <summary>
/// Validate số container theo chuẩn ISO 6346.
/// 
/// Cấu trúc: 4 chữ cái + 7 chữ số (tổng 11 ký tự)
///   - 3 chữ cái đầu: Owner Code (hãng khai thác)
///   - 1 chữ cái tiếp theo: Type Code (loại container)
///   - 6 chữ số: Serial Number
///   - 1 chữ số cuối (hoặc X): Check Digit theo Modulo 11
/// 
/// Ví dụ: CMAU1234567
///   CMA = Owner Code
///   U   = Type Code (Dry container)
///   123456 = Serial Number
///   7 = Check Digit
/// 
/// Thuật toán Modulo 11:
///   1. Gán trọng số 2^n cho từng ký tự (ký tự đầu có trọng số cao nhất)
///   2. Tính tổng các tích (ký tự * trọng số)
///   3. Lấy tổng chia 11 lấy dư
///   4. Nếu dư = 0 -> check digit = 0
///      Nếu dư từ 1 đến 9 -> check digit = dư
///      Nếu dư = 10 -> check digit = X
/// 
/// Mapping giá trị chữ cái sang số:
///   A=10, B=12, C=13, D=14, ..., Z=38 (bỏ qua 11, 22, 33, 44...)
///   0=0, 1=1, ..., 9=9
/// </summary>
public static class ContainerNumberValidator
{
    private static readonly Regex ContainerNumberPattern = new(@"^[A-Z]{4}\d{7}$", RegexOptions.Compiled);

    /// <summary>
    /// Map chữ cái -> số theo ISO 6346 (bỏ qua bội số của 11).
    /// </summary>
    private static int LetterToNumber(char c)
    {
        // Theo ISO 6346: A=10, B=12, C=13, ..., Z=38
        // Bỏ qua 11, 22, 33 vì chúng là bội số của 11
        if (c < 'A' || c > 'Z')
            throw new ArgumentException($"Ký tự '{c}' không phải chữ cái hợp lệ.", nameof(c));

        int baseValue = (c - 'A') + 10; // A=10, B=11, C=12...
        // Nếu là bội số của 11 thì nhảy lên +1 (để bỏ qua 11, 22, 33, 44)
        // B (11) -> 12, K (21) -> 22, V (32) -> 33... 
        // Công thức: nếu baseValue % 11 == 0 thì baseValue + 1
        if (baseValue % 11 == 0)
            baseValue += 1;

        return baseValue;
    }

    /// <summary>
    /// Tính check digit từ 10 ký tự đầu (4 chữ cái + 6 số).
    /// </summary>
    /// <param name="containerNumberWithoutCheck">10 ký tự đầu của số container</param>
    /// <returns>Check digit ('0'-'9' hoặc 'X')</returns>
    public static char CalculateCheckDigit(string containerNumberWithoutCheck)
    {
        if (string.IsNullOrWhiteSpace(containerNumberWithoutCheck))
            throw new ArgumentException("Số container không được để trống.", nameof(containerNumberWithoutCheck));
        if (containerNumberWithoutCheck.Length != 10)
            throw new ArgumentException("Số container phải có đúng 10 ký tự (không bao gồm check digit).", nameof(containerNumberWithoutCheck));

        // Trọng số giảm dần từ 2^10 xuống 2^1
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            char c = containerNumberWithoutCheck[i];
            int value;

            if (char.IsLetter(c))
            {
                value = LetterToNumber(char.ToUpper(c));
            }
            else if (char.IsDigit(c))
            {
                value = c - '0';
            }
            else
            {
                throw new ArgumentException($"Ký tự '{c}' không hợp lệ tại vị trí {i}.", nameof(containerNumberWithoutCheck));
            }

            // Trọng số = 2^(10-i)
            int weight = (int)Math.Pow(2, 10 - i);
            sum += value * weight;
        }

        int remainder = sum % 11;
        int checkDigitValue = remainder == 10 ? 0 : remainder;

        return checkDigitValue == 10 ? 'X' : checkDigitValue.ToString()[0];
    }

    /// <summary>
    /// Validate số container đầy đủ 11 ký tự.
    /// </summary>
    /// <returns>true nếu hợp lệ, false nếu không</returns>
    public static bool IsValid(string? containerNumber)
    {
        if (string.IsNullOrWhiteSpace(containerNumber))
            return false;

        containerNumber = containerNumber.ToUpper().Trim();

        // Phải đúng 11 ký tự và đúng định dạng
        if (!ContainerNumberPattern.IsMatch(containerNumber))
            return false;

        // Tính check digit từ 10 ký tự đầu
        char expectedCheckDigit = CalculateCheckDigit(containerNumber.Substring(0, 10));
        char actualCheckDigit = containerNumber[10];

        return expectedCheckDigit == actualCheckDigit;
    }

    /// <summary>
    /// Validate và trả về lỗi chi tiết (nếu có).
    /// Áp dụng Action/Func delegate.
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateWithMessage(string? containerNumber)
    {
        if (string.IsNullOrWhiteSpace(containerNumber))
            return (false, "Số container không được để trống.");

        containerNumber = containerNumber.ToUpper().Trim();

        if (containerNumber.Length != 11)
            return (false, $"Số container phải có đúng 11 ký tự (hiện tại: {containerNumber.Length}).");

        if (!ContainerNumberPattern.IsMatch(containerNumber))
            return (false, "Số container phải có định dạng: 4 chữ cái đầu + 7 chữ số sau.");

        char expectedCheckDigit = CalculateCheckDigit(containerNumber.Substring(0, 10));
        char actualCheckDigit = containerNumber[10];

        if (expectedCheckDigit != actualCheckDigit)
            return (false, $"Check digit không hợp lệ. Mong đợi '{expectedCheckDigit}' nhưng nhận '{actualCheckDigit}'.");

        return (true, null);
    }

    /// <summary>
    /// Trích xuất thông tin từ số container.
    /// </summary>
    public static ContainerNumberInfo Parse(string containerNumber)
    {
        containerNumber = containerNumber.ToUpper().Trim();
        return new ContainerNumberInfo
        {
            OwnerCode = containerNumber.Substring(0, 3),
            CategoryIdentifier = containerNumber[3],
            SerialNumber = containerNumber.Substring(4, 6),
            CheckDigit = containerNumber[10]
        };
    }
}

/// <summary>
/// Thông tin được tách ra từ số container.
/// </summary>
public class ContainerNumberInfo
{
    /// <summary>Mã chủ sở hữu (3 chữ cái đầu)</summary>
    public string OwnerCode { get; set; } = string.Empty;

    /// <summary>Mã phân loại container (1 chữ cái)</summary>
    public char CategoryIdentifier { get; set; }

    /// <summary>Số dãy (6 chữ số)</summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>Check digit (chữ số hoặc X)</summary>
    public char CheckDigit { get; set; }

    public override string ToString() => $"{OwnerCode}{CategoryIdentifier}{SerialNumber}{CheckDigit}";
}
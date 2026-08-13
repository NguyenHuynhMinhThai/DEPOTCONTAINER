using DEPOTCONTAINER.Models.Entities;

namespace DEPOTCONTAINER.Pages.Shared;

/// <summary>
/// Base PageModel cho tất cả Razor Pages.
/// Cung cấp các helper chung và ViewModels.
/// </summary>
public abstract class BasePageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
{
    /// <summary>Hiển thị thông báo thành công</summary>
    protected void SetSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    /// <summary>Hiển thị thông báo lỗi</summary>
    protected void SetErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }
}
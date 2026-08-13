using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Customers;

public class DeleteModel : PageModel
{
    private readonly ICustomerService _service;

    public DeleteModel(ICustomerService service) => _service = service;

    [BindProperty]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();
        Id = id;
        Name = result.Data.Name;
        TaxCode = result.Data.TaxCode;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _service.DeleteAsync(Id);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã xóa";
            return RedirectToPage("Index");
        }
        TempData["ErrorMessage"] = result.Message;
        return RedirectToPage("Index");
    }
}
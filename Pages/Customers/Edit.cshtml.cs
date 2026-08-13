using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Customers;

public class EditModel : PageModel
{
    private readonly ICustomerService _service;

    public EditModel(ICustomerService service) => _service = service;

    [BindProperty]
    public CustomerDto Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();
        Input = result.Data;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _service.UpdateAsync(Input.Id, Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã cập nhật";
            return RedirectToPage("Index");
        }
        ModelState.AddModelError("", result.Message);
        return Page();
    }
}
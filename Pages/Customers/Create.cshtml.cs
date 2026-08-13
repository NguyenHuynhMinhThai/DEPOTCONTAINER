using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Customers;

public class CreateModel : PageModel
{
    private readonly ICustomerService _service;

    public CreateModel(ICustomerService service) => _service = service;

    [BindProperty]
    public CustomerDto Input { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _service.CreateAsync(Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã tạo khách hàng";
            return RedirectToPage("Index");
        }
        ModelState.AddModelError("", result.Message);
        return Page();
    }
}
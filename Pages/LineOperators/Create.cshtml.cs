using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.LineOperators;

public class CreateModel : PageModel
{
    private readonly ILineOperatorService _service;

    public CreateModel(ILineOperatorService service) => _service = service;

    [BindProperty]
    public LineOperatorDto Input { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _service.CreateAsync(Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã tạo Line Operator";
            return RedirectToPage("Index");
        }
        ModelState.AddModelError("", result.Message);
        return Page();
    }
}
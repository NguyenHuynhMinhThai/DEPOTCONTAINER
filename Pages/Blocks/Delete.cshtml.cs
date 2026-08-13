using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Blocks;

public class DeleteModel : PageModel
{
    private readonly IBlockService _service;

    public DeleteModel(IBlockService service) => _service = service;

    [BindProperty]
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();
        Id = id;
        Code = result.Data.Code;
        Name = result.Data.Name;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _service.DeleteAsync(Id);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã xóa block.";
            return RedirectToPage("Index");
        }
        TempData["ErrorMessage"] = result.Message;
        return RedirectToPage("Index");
    }
}
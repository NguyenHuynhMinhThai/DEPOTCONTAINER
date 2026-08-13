using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Containers;

public class DeleteModel : PageModel
{
    private readonly IContainerService _service;

    public DeleteModel(IContainerService service) => _service = service;

    [BindProperty]
    public int Id { get; set; }

    public ContainerDto? Container { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();
        Container = result.Data;
        Id = id;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _service.DeleteAsync(Id);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã xóa container.";
            return RedirectToPage("Index");
        }
        TempData["ErrorMessage"] = result.Message;
        return RedirectToPage("Index");
    }
}
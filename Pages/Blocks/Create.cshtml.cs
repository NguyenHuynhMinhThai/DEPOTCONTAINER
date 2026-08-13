using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPOTCONTAINER.Pages.Blocks;

public class CreateModel : PageModel
{
    private readonly IBlockService _service;

    public CreateModel(IBlockService service) => _service = service;

    [BindProperty]
    public CreateBlockDto Input { get; set; } = new();

    public SelectList BlockTypes { get; set; } = null!;
    public SelectList Sizes { get; set; } = null!;

    public void OnGet()
    {
        BlockTypes = new SelectList(Enum.GetValues<BlockType>(), Input.BlockType);
        Sizes = new SelectList(
            Enum.GetValues<ContainerSize>().Select(s => new { Value = (int)s, Text = $"{(int)s} feet" }),
            "Value", "Text");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            OnGet();
            return Page();
        }

        var result = await _service.CreateAsync(Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = $"Đã tạo block {result.Data?.Code}";
            return RedirectToPage("Index");
        }

        ModelState.AddModelError("", result.Message);
        OnGet();
        return Page();
    }
}
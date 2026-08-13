using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPOTCONTAINER.Pages.Containers;

public class CreateModel : PageModel
{
    private readonly IContainerService _containerService;
    private readonly ILineOperatorService _lineOperatorService;

    public CreateModel(IContainerService containerService, ILineOperatorService lineOperatorService)
    {
        _containerService = containerService;
        _lineOperatorService = lineOperatorService;
    }

    [BindProperty]
    public CreateContainerDto Input { get; set; } = new();

    public SelectList ContainerTypes { get; set; } = null!;
    public SelectList Sizes { get; set; } = null!;
    public SelectList Conditions { get; set; } = null!;
    public SelectList Categories { get; set; } = null!;
    public SelectList LineOperators { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        var result = await _containerService.CreateAsync(Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = $"Tạo container {result.Data?.ContainerNumber} thành công!";
            return RedirectToPage("Index");
        }

        ModelState.AddModelError("", result.Message);
        if (result.Errors.Any())
            foreach (var err in result.Errors)
                ModelState.AddModelError("", err);
        await LoadSelectListsAsync();
        return Page();
    }

    private async Task LoadSelectListsAsync()
    {
        ContainerTypes = new SelectList(Enum.GetValues<ContainerType>().Select(x => new { Value = (int)x, Text = x.ToString() }), "Value", "Text");
        Sizes = new SelectList(Enum.GetValues<ContainerSize>().Select(x => new { Value = (int)x, Text = $"{(int)x} feet" }), "Value", "Text");
        Conditions = new SelectList(Enum.GetValues<ContainerCondition>(), Input.Condition);
        Categories = new SelectList(Enum.GetValues<ContainerCategory>(), Input.Category);

        var opsResult = await _lineOperatorService.GetPagedAsync(new QueryParameters { PageSize = 100 });
        LineOperators = new SelectList(opsResult.Data?.Items ?? new List<LineOperatorDto>(), "Id", "Name");
    }
}
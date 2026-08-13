using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPOTCONTAINER.Pages.Containers;

public class EditModel : PageModel
{
    private readonly IContainerService _containerService;
    private readonly ILineOperatorService _lineOperatorService;

    public EditModel(IContainerService containerService, ILineOperatorService lineOperatorService)
    {
        _containerService = containerService;
        _lineOperatorService = lineOperatorService;
    }

    [BindProperty]
    public UpdateContainerDto Input { get; set; } = new();

    public ContainerDto? Container { get; set; }
    public SelectList ContainerTypes { get; set; } = null!;
    public SelectList Sizes { get; set; } = null!;
    public SelectList Conditions { get; set; } = null!;
    public SelectList Categories { get; set; } = null!;
    public SelectList LineOperators { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await _containerService.GetByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();
        Container = result.Data;

        Input = new UpdateContainerDto
        {
            ContainerType = Container.ContainerType,
            IsoCode = Container.IsoCode,
            Size = Container.Size,
            MaxWeight = Container.MaxWeight,
            TareWeight = Container.TareWeight,
            ManufactureDate = Container.ManufactureDate,
            LineOperatorId = Container.LineOperatorId,
            Condition = Container.Condition,
            Category = Container.Category,
            DamageDescription = Container.DamageDescription
        };

        await LoadSelectLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return Page();
        }

        var result = await _containerService.UpdateAsync(id, Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Cập nhật thành công!";
            return RedirectToPage("Index");
        }

        ModelState.AddModelError("", result.Message);
        await LoadSelectLists();
        return Page();
    }

    private async Task LoadSelectLists()
    {
        ContainerTypes = new SelectList(Enum.GetValues<ContainerType>(), Input.ContainerType);
        Sizes = new SelectList(Enum.GetValues<ContainerSize>(), Input.Size);
        Conditions = new SelectList(Enum.GetValues<ContainerCondition>(), Input.Condition);
        Categories = new SelectList(Enum.GetValues<ContainerCategory>(), Input.Category);

        var opsResult = await _lineOperatorService.GetPagedAsync(new QueryParameters { PageSize = 100 });
        LineOperators = new SelectList(opsResult.Data?.Items ?? new List<LineOperatorDto>(), "Id", "Name");
    }
}
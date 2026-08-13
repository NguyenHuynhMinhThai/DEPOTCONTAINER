using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Services.Interfaces;
using DEPOTCONTAINER.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPOTCONTAINER.Pages.Movements;

public class CreateModel : PageModel
{
    private readonly IContainerMovementService _movementService;
    private readonly IContainerService _containerService;
    private readonly IBlockService _blockService;

    public CreateModel(
        IContainerMovementService movementService,
        IContainerService containerService,
        IBlockService blockService)
    {
        _movementService = movementService;
        _containerService = containerService;
        _blockService = blockService;
    }

    [BindProperty]
    public CreateMovementDto Input { get; set; } = new();

    [BindProperty]
    public string ContainerNumber { get; set; } = string.Empty;

    public SelectList MovementTypes { get; set; } = null!;
    public SelectList Blocks { get; set; } = null!;

    public async Task OnGetAsync()
    {
        MovementTypes = new SelectList(
            Enum.GetValues<MovementType>().Select(x => new { Value = (int)x, Text = x.ToString() }),
            "Value", "Text");

        var blocksResult = await _blockService.GetPagedAsync(new QueryParameters { PageSize = 100 });
        Blocks = new SelectList(blocksResult.Data?.Items ?? new List<BlockDto>(), "Id", "Code");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var (valid, err) = ContainerNumberValidator.ValidateWithMessage(ContainerNumber);
        if (!valid)
        {
            ModelState.AddModelError("ContainerNumber", err ?? "Số container không hợp lệ");
            await OnGetAsync();
            return Page();
        }

        var c = await _containerService.GetByContainerNumberAsync(ContainerNumber);
        if (!c.Success || c.Data == null)
        {
            ModelState.AddModelError("ContainerNumber", "Không tìm thấy container trong hệ thống");
            await OnGetAsync();
            return Page();
        }

        Input.ContainerId = c.Data.Id;
        var result = await _movementService.CreateAsync(Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã ghi nhận movement!";
            return RedirectToPage("Index");
        }

        ModelState.AddModelError("", result.Message);
        await OnGetAsync();
        return Page();
    }
}
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPOTCONTAINER.Pages.ReleaseOrders;

public class CreateModel : PageModel
{
    private readonly IReleaseOrderService _releaseOrderService;
    private readonly ILineOperatorService _lineOperatorService;
    private readonly ICustomerService _customerService;

    public CreateModel(
        IReleaseOrderService releaseOrderService,
        ILineOperatorService lineOperatorService,
        ICustomerService customerService)
    {
        _releaseOrderService = releaseOrderService;
        _lineOperatorService = lineOperatorService;
        _customerService = customerService;
    }

    [BindProperty]
    public CreateReleaseOrderDto Input { get; set; } = new()
    {
        Details = new List<CreateReleaseOrderDetailDto>
        {
            new() { ContainerSize = ContainerSize.Size20, ContainerType = ContainerType.Dry, Quantity = 10 },
            new() { ContainerSize = ContainerSize.Size40, ContainerType = ContainerType.Dry, Quantity = 5 }
        }
    };

    public SelectList LineOperators { get; set; } = null!;
    public SelectList Customers { get; set; } = null!;
    public SelectList Sizes { get; set; } = null!;
    public SelectList ContainerTypes { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadSelectLists();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return Page();
        }

        var result = await _releaseOrderService.CreateAsync(Input);
        if (result.Success)
        {
            TempData["SuccessMessage"] = $"Đã tạo lệnh {result.Data?.OrderNumber}";
            return RedirectToPage("Index");
        }

        ModelState.AddModelError("", result.Message);
        await LoadSelectLists();
        return Page();
    }

    private async Task LoadSelectLists()
    {
        var ops = await _lineOperatorService.GetPagedAsync(new QueryParameters { PageSize = 100 });
        LineOperators = new SelectList(ops.Data?.Items ?? new List<LineOperatorDto>(), "Id", "Name");

        var customers = await _customerService.GetPagedAsync(new QueryParameters { PageSize = 100 });
        Customers = new SelectList(customers.Data?.Items ?? new List<CustomerDto>(), "Id", "Name");

        Sizes = new SelectList(
            Enum.GetValues<ContainerSize>().Select(s => new { Value = (int)s, Text = $"{(int)s} feet" }),
            "Value", "Text");
        ContainerTypes = new SelectList(
            Enum.GetValues<ContainerType>().Select(t => new { Value = (int)t, Text = t.ToString() }),
            "Value", "Text");
    }
}
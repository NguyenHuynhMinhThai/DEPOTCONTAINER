using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.ReleaseOrders;

public class IndexModel : PageModel
{
    private readonly IReleaseOrderService _service;

    public IndexModel(IReleaseOrderService service) => _service = service;

    public PagedResult<ReleaseOrderDto> PagedData { get; set; } = PagedResult<ReleaseOrderDto>.Empty();

    public async Task OnGetAsync()
    {
        var result = await _service.GetPagedAsync(new QueryParameters { PageSize = 50 });
        if (result.Success && result.Data != null) PagedData = result.Data;
    }
}
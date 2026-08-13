using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Movements;

public class IndexModel : PageModel
{
    private readonly IContainerMovementService _service;

    public IndexModel(IContainerMovementService service) => _service = service;

    public PagedResult<ContainerMovementDto> PagedData { get; set; } = PagedResult<ContainerMovementDto>.Empty();

    public async Task OnGetAsync()
    {
        var result = await _service.GetPagedAsync(new QueryParameters { PageSize = 50 });
        if (result.Success && result.Data != null) PagedData = result.Data;
    }
}
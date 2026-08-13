using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Containers;

public class IndexModel : PageModel
{
    private readonly IContainerService _service;

    public IndexModel(IContainerService service) => _service = service;

    public PagedResult<ContainerDto> PagedData { get; set; } = PagedResult<ContainerDto>.Empty();
    public QueryParameters Parameters { get; set; } = new();

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? searchTerm = null)
    {
        Parameters = new QueryParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SearchTerm = searchTerm
        };

        var result = await _service.GetPagedAsync(Parameters);
        if (result.Success && result.Data != null)
            PagedData = result.Data;
    }
}
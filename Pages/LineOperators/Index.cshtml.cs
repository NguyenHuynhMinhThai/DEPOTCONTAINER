using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.LineOperators;

public class IndexModel : PageModel
{
    private readonly ILineOperatorService _service;

    public IndexModel(ILineOperatorService service) => _service = service;

    public PagedResult<LineOperatorDto> PagedData { get; set; } = PagedResult<LineOperatorDto>.Empty();

    public async Task OnGetAsync()
    {
        var result = await _service.GetPagedAsync(new QueryParameters { PageSize = 100 });
        if (result.Success && result.Data != null) PagedData = result.Data;
    }
}
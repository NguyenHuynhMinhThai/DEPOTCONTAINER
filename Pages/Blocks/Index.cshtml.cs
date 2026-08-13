using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Blocks;

public class IndexModel : PageModel
{
    private readonly IBlockService _service;

    public IndexModel(IBlockService service) => _service = service;

    public PagedResult<BlockDto> PagedData { get; set; } = PagedResult<BlockDto>.Empty();

    public async Task OnGetAsync()
    {
        var result = await _service.GetPagedAsync(new QueryParameters { PageSize = 100 });
        if (result.Success && result.Data != null) PagedData = result.Data;
    }
}
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Blocks;

public class LayoutModel : PageModel
{
    private readonly IBlockService _service;

    public LayoutModel(IBlockService service) => _service = service;

    public BlockDto? Block { get; set; }
    public BlockLayoutDto? Layout { get; set; }
    public int TierCount { get; set; }
    public int TotalContainers { get; set; }

    public async Task OnGetAsync(int id)
    {
        var blockResult = await _service.GetByIdAsync(id);
        Block = blockResult.Data;

        var layoutResult = await _service.GetBlockLayoutAsync(id);
        if (layoutResult.Success && layoutResult.Data != null)
        {
            Layout = layoutResult.Data;
            TierCount = Layout.Bays.SelectMany(b => b.Rows).SelectMany(r => r.Tiers).Select(t => t.TierNumber).DefaultIfEmpty(0).Max();
            TotalContainers = Layout.Bays.SelectMany(b => b.Rows).SelectMany(r => r.Tiers).Count(t => t.IsOccupied);
        }
    }
}
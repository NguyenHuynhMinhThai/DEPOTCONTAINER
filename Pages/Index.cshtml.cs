using DEPOTCONTAINER.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages;

public class IndexModel : PageModel
{
    private readonly IUnitOfWork _uow;

    public IndexModel(IUnitOfWork uow) => _uow = uow;

    public int TotalBlocks { get; set; }
    public int TotalContainers { get; set; }
    public int TotalMovements { get; set; }

    public async Task OnGetAsync()
    {
        TotalBlocks = await _uow.Blocks.CountAsync();
        TotalContainers = await _uow.Containers.CountAsync();
        TotalMovements = await _uow.ContainerMovements.CountAsync();
    }
}
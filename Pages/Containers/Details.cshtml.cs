using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Containers;

public class DetailsModel : PageModel
{
    private readonly IContainerService _service;

    public DetailsModel(IContainerService service) => _service = service;

    public ContainerDto? Container { get; set; }

    public async Task OnGetAsync(int id)
    {
        var result = await _service.GetByIdAsync(id);
        Container = result.Success ? result.Data : null;
    }
}
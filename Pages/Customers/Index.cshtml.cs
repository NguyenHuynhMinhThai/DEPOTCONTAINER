using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEPOTCONTAINER.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly ICustomerService _service;

    public IndexModel(ICustomerService service) => _service = service;

    public PagedResult<CustomerDto> PagedData { get; set; } = PagedResult<CustomerDto>.Empty();

    public async Task OnGetAsync()
    {
        var result = await _service.GetPagedAsync(new QueryParameters { PageSize = 100 });
        if (result.Success && result.Data != null) PagedData = result.Data;
    }
}
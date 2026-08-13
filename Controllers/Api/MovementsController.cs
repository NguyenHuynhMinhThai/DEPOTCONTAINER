using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DEPOTCONTAINER.Controllers.Api;

[ApiController]
[Route("api/movements")]
public class MovementsController : ControllerBase
{
    private readonly IContainerMovementService _service;

    public MovementsController(IContainerMovementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] QueryParameters parameters)
    {
        return Ok(await _service.GetPagedAsync(parameters));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("by-container/{containerId:int}")]
    public async Task<IActionResult> GetByContainer(int containerId)
    {
        var result = await _service.GetMovementsByContainerAsync(containerId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMovementDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result)
            : BadRequest(result);
    }
}

[ApiController]
[Route("api/release-orders")]
public class ReleaseOrdersController : ControllerBase
{
    private readonly IReleaseOrderService _service;

    public ReleaseOrdersController(IReleaseOrderService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] QueryParameters parameters)
    {
        return Ok(await _service.GetPagedAsync(parameters));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReleaseOrderDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result)
            : BadRequest(result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var result = await _service.UpdateStatusAsync(id, request.Status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Thực hiện lệnh giao container (xuất container khỏi bãi theo lệnh)</summary>
    [HttpPost("{id:int}/execute")]
    public async Task<IActionResult> ExecuteRelease(int id, [FromBody] ExecuteReleaseRequest request)
    {
        var result = await _service.ExecuteReleaseAsync(id, request.ContainerId, request.Vehicle, request.DriverName);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public class UpdateStatusRequest
{
    public Models.Enums.ReleaseOrderStatus Status { get; set; }
}

public class ExecuteReleaseRequest
{
    public int ContainerId { get; set; }
    public string Vehicle { get; set; } = string.Empty;
    public string? DriverName { get; set; }
}
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DEPOTCONTAINER.Controllers.Api;

/// <summary>
/// API Controller cho Container - endpoints REST.
/// Route: /api/containers
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContainersController : ControllerBase
{
    private readonly IContainerService _service;

    public ContainersController(IContainerService service)
    {
        _service = service;
    }

    /// <summary>Lấy danh sách container có phân trang</summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] QueryParameters parameters)
    {
        var result = await _service.GetPagedAsync(parameters);
        return Ok(result);
    }

    /// <summary>Lấy container theo Id</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Lấy container theo số container</summary>
    [HttpGet("by-number/{containerNumber}")]
    public async Task<IActionResult> GetByNumber(string containerNumber)
    {
        var result = await _service.GetByContainerNumberAsync(containerNumber);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Tạo mới container</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContainerDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Cập nhật container</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateContainerDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Xóa container</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Validate Container Number theo Modulo 11</summary>
    [HttpGet("validate/{containerNumber}")]
    public async Task<IActionResult> ValidateContainerNumber(string containerNumber)
    {
        var result = await _service.ValidateContainerNumberAsync(containerNumber);
        return Ok(result);
    }

    /// <summary>Gán vị trí cho container trong bãi</summary>
    [HttpPost("{id:int}/assign-location")]
    public async Task<IActionResult> AssignLocation(int id, [FromBody] AssignLocationRequest request)
    {
        var result = await _service.AssignLocationAsync(id, request.BlockId, request.BayId, request.RowId, request.TierId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

/// <summary>Request body cho AssignLocation</summary>
public class AssignLocationRequest
{
    public int? BlockId { get; set; }
    public int? BayId { get; set; }
    public int? RowId { get; set; }
    public int? TierId { get; set; }
}
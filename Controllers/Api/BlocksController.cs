using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DEPOTCONTAINER.Controllers.Api;

/// <summary>
/// API Controller cho Block - quản lý bãi container.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BlocksController : ControllerBase
{
    private readonly IBlockService _service;

    public BlocksController(IBlockService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] QueryParameters parameters)
    {
        var result = await _service.GetPagedAsync(parameters);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:int}/layout")]
    public async Task<IActionResult> GetLayout(int id)
    {
        var result = await _service.GetBlockLayoutAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBlockDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result)
            : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateBlockDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/generate-layout")]
    public async Task<IActionResult> GenerateLayout(int id)
    {
        var result = await _service.GenerateLayoutAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
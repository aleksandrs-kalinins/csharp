using Microsoft.AspNetCore.Mvc;
using ConcurrentApi.Services;

namespace ConcurrentApi.Controllers;

[ApiController]
[Route("api/control")]
public class ControlController : ControllerBase
{
    private readonly DualTaskService _service;

    public ControlController(DualTaskService service)
    {
        _service = service;
    }

    [HttpPost("start")]
    public IActionResult Start()
    {
        _service.StartWork(HttpContext.RequestAborted);
        return Ok("Tasks started");
    }

    [HttpPost("stop")]
    public async Task<IActionResult> Stop()
    {
        Console.WriteLine(">>> STOP endpoint called");
        await _service.StopWorkAsync();
        return Ok("Tasks stopped");
    }
}
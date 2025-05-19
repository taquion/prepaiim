using Microsoft.AspNetCore.Mvc;

namespace PreparatoriaIIM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { 
            Status = "Success",
            Message = "¡API desplegada exitosamente en Azure!" 
        });
    }
}

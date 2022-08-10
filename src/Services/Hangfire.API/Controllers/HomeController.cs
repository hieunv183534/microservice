using Microsoft.AspNetCore.Mvc;

namespace Hangfire.API.Controllers;

public class HomeController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return Redirect("~/jobs");
    }
}
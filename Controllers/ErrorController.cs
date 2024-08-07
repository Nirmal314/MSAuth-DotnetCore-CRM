using Microsoft.AspNetCore.Mvc;

namespace MSAuth.Controllers;

public class ErrorController : Controller
{
    public IActionResult _403()
    {
        return View();
    }
    public IActionResult _500()
    {
        return View();
    }
    public IActionResult _404()
    {
        return View();
    }
    public IActionResult _503()
    {
        return View();
    }

}

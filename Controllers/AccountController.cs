using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSAuth.Interfaces;
using MSAuth.Models;
namespace MSAUth.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public async Task<IActionResult> SignOut()
    {
        HttpContext.Session.Clear();

        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        await HttpContext.SignOutAsync("Cookies");
        await HttpContext.SignOutAsync("OpenIdConnect");

        return Json(new { });
        //return Redirect("~/MicrosoftIdentity/Account/SignIn");
    }
}
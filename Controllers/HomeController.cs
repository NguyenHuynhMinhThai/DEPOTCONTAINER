using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DEPOTCONTAINER.Models;

namespace DEPOTCONTAINER.Controllers;

public class HomeController : Controller
{
    /// <summary>
    /// Redirect đến trang Razor Pages Index (/).
    /// </summary>
    public IActionResult Index()
    {
        return RedirectToPage("/Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
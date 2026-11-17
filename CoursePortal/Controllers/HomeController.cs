using System.Diagnostics;
using CoursePortal.Models;
using CoursePortal.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoursePortal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICourseRepository _courses;

    public HomeController(ILogger<HomeController> logger, ICourseRepository courses)
    {
        _logger = logger;
        _courses = courses;
    }

    // Landing page with courses from DB
    public async Task<IActionResult> Index()
    {
        var courses = await _courses.GetActiveCourses();
        return View(courses);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

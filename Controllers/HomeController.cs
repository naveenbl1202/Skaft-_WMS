using Microsoft.AspNetCore.Mvc;
using SkaftoBageriA.Models;
using System.Diagnostics;

namespace SkaftoBageriA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

     
    }
}

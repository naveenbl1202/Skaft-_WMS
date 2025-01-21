using Microsoft.AspNetCore.Mvc;

namespace SkaftoBageriA.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

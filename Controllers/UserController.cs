using Microsoft.AspNetCore.Mvc;

namespace SkaftoBageriA.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

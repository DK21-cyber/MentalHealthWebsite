using Microsoft.AspNetCore.Mvc;

namespace PW.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

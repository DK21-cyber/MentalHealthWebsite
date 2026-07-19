using Microsoft.AspNetCore.Mvc;

namespace PW.Areas.Admin.Controllers
{
    public class ResultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

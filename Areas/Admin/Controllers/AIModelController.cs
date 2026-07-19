using Microsoft.AspNetCore.Mvc;
using PW.Filters;

namespace PW.Areas.Admin.Controllers
{
    [SessionAuthorize("Admin")]
    public class AIModelController : Controller
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

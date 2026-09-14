using Microsoft.AspNetCore.Mvc;

namespace TravoRides.CMS.Controllers
{
    public class PackageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace TravoRides.CMS.Controllers
{
    public class SelfDriveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

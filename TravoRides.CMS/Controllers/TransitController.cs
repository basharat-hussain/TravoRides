using Microsoft.AspNetCore.Mvc;

namespace TravoRides.CMS.Controllers
{
    public class TransitController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

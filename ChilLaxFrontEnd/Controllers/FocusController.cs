using ChilLaxFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChilLaxFrontEnd.Controllers
{
    public class FocusController : Controller
    {
        ChilLaxContext db = new ChilLaxContext();
        public IActionResult Index()
        {
            var data = db.FocusSlide.ToList();
            return View(data);
        }

        public string EarnPoint()
        {
            return "";
        }
    }
}

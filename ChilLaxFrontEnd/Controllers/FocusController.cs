using ChilLaxFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChilLaxFrontEnd.Controllers
{
    public class FocusController : Controller
    {
        ChilLaxContext db = new ChilLaxContext();
        public IActionResult Index()
        {
            var data = db.FocusSlides.ToList();
            return View(data);
        }


    }
}

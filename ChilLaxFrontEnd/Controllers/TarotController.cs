using Microsoft.AspNetCore.Mvc;

namespace ChilLaxFrontEnd.Controllers
{
    public class TarotController : Controller
    {
        public IActionResult TarotIndex()
        {
            return View();
        }
    }
}

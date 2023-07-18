using ChilLaxFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChilLaxFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        ChilLaxContext _context = new ChilLaxContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var data = _context.Announcements.Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now);
            return View(data);
        }

        public IActionResult Mission()
        {
            return View();
        }

        public IActionResult Team()
        {
            return View();
        }

        public IActionResult Attribution()
        {
            return View();
        }

        public IActionResult PointRecord()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
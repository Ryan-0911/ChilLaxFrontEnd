using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChilLaxFrontEnd.Controllers
{
    public class Char : Controller
    {
        // GET: Char
        public ActionResult Index()
        {
            return View();
        }

        // GET: Char/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Char/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Char/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Char/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Char/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Char/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Char/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

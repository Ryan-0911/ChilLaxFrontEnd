using ChilLaxFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChilLaxFrontEnd.Controllers
{
    public class ProductController : Controller
    {
        ChilLaxContext db = new ChilLaxContext();
        private readonly ChilLaxContext _context;

        public ProductController(ChilLaxContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult List()
        {
            //string keyword;
            //if (Request.Form.TryGetValue("txtKeyword", out var keywordValue))
            //{
            //    keyword = keywordValue;
            //}
            //else
            //{
            //    keyword = null;
            //}

            //IEnumerable<Product> datas = null;
            //if (string.IsNullOrEmpty(keyword))
            //{
            //    datas = from p in db.Products
            //            select p;
            //}
            //else
            //{
            //    datas = db.Products.Where(p => p.ProductName.Contains(keyword));
            //}

            //return View(datas);

            var datas = from p in db.Product
                        select p;
            return View(datas);
        }

        public IActionResult Details() 
        {
            return View();
        }


    }
}

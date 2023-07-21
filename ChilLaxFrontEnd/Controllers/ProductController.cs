using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult List(CKeywordViewModel ckvm)
        {
            string keyword = ckvm.txtKeyword;
            IEnumerable<Product> datas = null;
            if (string.IsNullOrEmpty(keyword))
            {
                datas = from p in db.Products
                        select p;
            }
            else
            {
                datas = db.Products.Where(p => p.ProductName.Contains(keyword));
            }

            return View(datas);

            //var datas = from p in db.Products
            //            select p;
            //return View(datas);
        }

        public IActionResult Details()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    P_DetailViewModel pdvm = new P_DetailViewModel();

        //    var product = await _context.Products.Include(p => p.ProductCategory).FirstOrDefaultAsync(m => m.ProductId == id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        pdvm.product = product;
        //        if (product.ProductImg != null)
        //        {
        //            pdvm.ProductImg = ViewImage(product.ProductImg);
        //        }
        //    }
        //    return View(pdvm);
        //}

        //private string ViewImage(byte[] arrayImage)
        //{
        //    // 二進位圖檔轉字串
        //    string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
        //    return "data:image/png;base64," + base64String;
        //}
    }

}

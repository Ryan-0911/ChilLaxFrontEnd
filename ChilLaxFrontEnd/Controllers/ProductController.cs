using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.Models.DTO;
using ChilLaxFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq; // 加入這個命名空間以使用 LINQ 查詢
using System.Collections.Generic; // 加入這個命名空間以使用 IEnumerable<T>
using System.Threading.Tasks; // 加入這個命名空間以使用非同步 Task<T> 方法
using System;

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
            // 關鍵字搜尋
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


        }

       

        [HttpGet]
        public async Task<ActionResult<ProductsPagingDTO>> GetProductsByCategory(string category, int page = 1)
        {
            var productsInCategory = _context.Products.Where(p => p.ProductCategory == category).ToList();
            //return Json(productsInCategory);

            

            int PageSize = 8;

            // 計算商品總數量和總頁數
            int totalProducts = productsInCategory.Count();
            int totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);

            // 取得當前頁面的商品資料
            var currentPageProducts = productsInCategory
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // 回傳商品資料和分頁相關資訊
            ProductsPagingDTO prodDTO = new ProductsPagingDTO();
            prodDTO.TotalPages = totalPages;
            prodDTO.ProductsResult = currentPageProducts;

            return prodDTO;

        }


        public IActionResult AddToCart(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("List");
            }
            ViewBag.product_id = id;
            return View();
        }

        [HttpPost]
        public IActionResult AddToCart(CAddToCartViewModel cvm)
        {
            ChilLaxContext db = new ChilLaxContext();
            Product prod = db.Products.FirstOrDefault(t => t.ProductId == cvm.txtFId);
            if (prod != null)
            {
                string json = "";
                List<CShoppingCartItem> cart = null;
                if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
                {
                    json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                    cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
                }
                else
                    cart = new List<CShoppingCartItem>();
                CShoppingCartItem item = new CShoppingCartItem();
                item.price = (decimal)prod.ProductPrice;
                item.productId = cvm.txtFId;
                item.count = cvm.txtCount;
                item.product = prod;
                cart.Add(item);
                json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
            }
            return RedirectToAction("List");
        }


    }

}

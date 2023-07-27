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

        [HttpGet]
        public IActionResult List(CKeywordViewModel ckvm, int? nowpage, int? _pageCount, string? productcategory)
        {
            // 關鍵字搜尋
            string keyword = ckvm.txtKeyword;
            IEnumerable<Product> datas = null;

            ProductsPagingDTO productsPagingDTO = new ProductsPagingDTO();
            productsPagingDTO.ProdCategory = db.Product
                .GroupBy(p => p.ProductCategory)
                .Select(group => group.Key)
                .ToList();

            if (string.IsNullOrEmpty(keyword))
            {
                datas = from p in db.Product
                        select p;
            }
            else
            {
                datas = db.Product.Where(p => p.ProductName.Contains(keyword));
            }



            // 頁數
            if (nowpage == null)
            {
                nowpage = 1;
            }
            int? pageCount = _pageCount;

            if (_pageCount == null)
            {
                int dataCount = db.Product.Count();
                pageCount = dataCount / 8;
                if (dataCount % 8 != 0) pageCount++;
            }

            // 處理類別之分頁

            if(productcategory == null)
            {
                productsPagingDTO.ProductsResult = datas
                    .Skip(8 * ((int)nowpage - 1))
                    .Take(8)
                    .ToList();
                productsPagingDTO.pageCount = pageCount;
                productsPagingDTO.nowpage = nowpage;    
                return View(new List<ProductsPagingDTO> { productsPagingDTO });
            }

            var prod = db.Product.Where(p => p.ProductCategory == productcategory )
                                .OrderByDescending(p => p.ProductCategory)
                                .Skip(8*((int)nowpage)-1)
                                .Take(8)
                                .ToList();
            
            productsPagingDTO.ProductsResult = prod;
            productsPagingDTO.pageCount = pageCount;
            productsPagingDTO.nowpage = nowpage;

            // 取得產品的分類群組資料
            var productCategories = db.Product
                .GroupBy(p => p.ProductCategory)
                .Select(group => group.Key)
                .ToList();

            // 傳遞產品分類群組資料到View
            productsPagingDTO.ProdCategory = productCategories;

            //return View(productsPagingDTO);
            //return View(new List<ProductsPagingDTO> { productsPagingDTO });
            return PartialView("_ProductListPartialView", new List<ProductsPagingDTO> { productsPagingDTO });


        }





        public IActionResult AddToCart(int? id)
        {
            //if(id == null)
            //{
            //    return RedirectToAction("List");
            //}
            //ViewBag.product_id = id;
            //return View();

            //if (id == null)
            //{
            //    return RedirectToAction("List");
            //}

            // 假設您的資料庫內含有名為 "Products" 的資料表，並包含 ProductId 欄位用於查詢產品
            Product product = db.Product.FirstOrDefault(p => p.ProductId == id);

            if (product == null || id == null) 
            {
                // 若找不到對應的產品，重新導向至產品列表頁面或顯示錯誤訊息
                return RedirectToAction("List");
            }

            // 將查詢結果傳遞給檢視
            return View(product);
        }

        [HttpPost]
        public IActionResult AddToCart(CAddToCartViewModel cvm, int ProductId, int MemberId )
        {
            ChilLaxContext db = new ChilLaxContext();


            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER); // 抓會員id登入的session
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            Cart cart = new Cart
            {
                MemberId = member.MemberId,
                ProductId = cvm.ProductId,
                CartProductQuantity = cvm.txtCount
            };


            db.Cart.Add(cart);
            db.SaveChanges();

            return View();
            //return RedirectToAction("AddToCart");


        }

        //[HttpPost]
        //public IActionResult ProductToCart(CAddToCartViewModel cvm)
        //{
        //    ChilLaxContext db = new ChilLaxContext();


        //    string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER); // 抓會員id登入的session
        //    Console.WriteLine(json);
        //    Member member = JsonSerializer.Deserialize<Member>(json);

        //    Cart cart = new Cart();

        //    cart.MemberId = member.MemberId;
        //    cart.ProductId = cvm.ProductId;
        //    cart.CartProductQuantity = cvm.txtCount;

        //    _context.Cart.Add(cart);
        //    _context.SaveChanges();
        //    //  await _context.SaveChangesAsync();   將暫存的異動儲存到資料庫，確保資料庫中的資料與內存中的資料保持同步。


        //    return RedirectToAction("List");

        //}

        // 檢視購物車
        public IActionResult CartView()
        {
            if(!HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                return RedirectToAction("List");
            }

            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            List<CShoppingCartItem> cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
            if (cart == null) 
                return RedirectToAction("List");
            return View(cart);

        }


    }

}

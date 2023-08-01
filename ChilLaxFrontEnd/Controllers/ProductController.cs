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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http;
using System.Text;
using static ChilLaxFrontEnd.Models.DTO.ProductsPagingDTO;

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
        public IActionResult List(CKeywordViewModel ckvm, CAddToCartViewModel cvm, int? nowpage, int? _pageCount, string? productcategory, int? likedProductId, string? likedProductName, string? likedProductImg, int? likedProductPrice, Cart carts)
        {
            //取得會員ID
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);
            int id = member.MemberId;

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

            //取的購物車商品
            //
            productsPagingDTO.carts = _context.Cart.Where(c => c.MemberId == id).ToList();

            // 取得購物車商品與商品項目的聯結結果
            List<CartProductItem> cartList = _context.Cart
                .Where(c => c.MemberId == id)
                .Join(_context.Product,
                    c => c.ProductId,
                    p => p.ProductId,
                    (c, p) => new CartProductItem
                    {
                        cartList = new List<Cart> { c },
                        products = new List<Product> { p }
                    }).ToList();

            // 將購物車商品與商品項目存入 productsPagingDTO 的 CartListItem 屬性
            productsPagingDTO.CartListItem = cartList;



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

            if (productcategory == null)
            {
                productsPagingDTO.ProductsResult = datas
                    .Skip(8 * ((int)nowpage - 1))
                    .Take(8)
                    .ToList();
                productsPagingDTO.pageCount = pageCount;
                productsPagingDTO.nowpage = nowpage;

                return View(new List<ProductsPagingDTO> { productsPagingDTO });
            }

            var prod = db.Product.Where(p => p.ProductCategory == productcategory)
                                .OrderByDescending(p => p.ProductCategory)
                                .Skip(8 * ((int)nowpage) - 1)
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


            // 寫入購物車資料表
            // 檢查資料庫中是否已經有相同的購物車記錄
            var existingCart = db.Cart.FirstOrDefault(c => c.MemberId == member.MemberId && c.ProductId == cvm.ProductId);

            if (existingCart != null)
            {
                // 如果已經存在相同的購物車記錄，可以選擇更新數量或是拋出錯誤訊息
                existingCart.CartProductQuantity += cvm.txtCount;
            }
            else
            {
                // 如果資料庫中還不存在相同的購物車記錄，則新增一筆新的購物車記錄
                Cart cart = new Cart
                {
                    MemberId = member.MemberId,
                    ProductId = cvm.ProductId,
                    CartProductQuantity = cvm.txtCount
                };

                db.Cart.Add(cart);
            }

            db.SaveChanges();



            // 添加喜愛商品的處理
            if (likedProductId != null && likedProductName != null && likedProductImg != null && likedProductPrice != null)
            {

                string pJson = HttpContext.Session.GetString(CDictionary.SK_LIKED_PRODUCTS_LIST);

                List<LikedProductDTO> likedProductsList = JsonSerializer.Deserialize<List<LikedProductDTO>>(pJson);

                if (likedProductsList == null)
                {
                    // 如果清單還不存在，創建一個新的清單
                    likedProductsList = new List<LikedProductDTO>();
                }

                // 創建一個新的喜愛商品對象
                var likedProduct = new LikedProductDTO
                {
                    ProductId = likedProductId.Value,
                    ProductName = likedProductName,
                    ProductImg = likedProductImg,
                    ProductPrice = likedProductPrice.Value
                };

                // 將新的喜愛商品對象添加到喜愛商品清單中
                likedProductsList.Add(likedProduct);

                // 將更新後的likedProductsList清單轉換成JSON格式的字串
                string updatedJson = JsonSerializer.Serialize(likedProductsList);

                // 將JSON字串轉換為byte[]數據並保存到HttpContext.Session中
                byte[] bytes = Encoding.UTF8.GetBytes(updatedJson);
                HttpContext.Session.Set(CDictionary.SK_LIKED_PRODUCTS_LIST, bytes);
            }


            // 新增購物車div中列表
            // 取得購物車商品
            List<CShoppingCartItem> cartItems = null;
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                string cartJson = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                cartItems = JsonSerializer.Deserialize<List<CShoppingCartItem>>(cartJson);
            }

            // 將購物車商品資訊傳遞到View中
            ViewBag.CartItems = cartItems;


            return View();


        }





        public IActionResult AddToCart(int? id)
        {

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
        public IActionResult AddToCart(CAddToCartViewModel cvm, int ProductId, int MemberId, int CartProductQuantity)
        {
            ChilLaxContext db = new ChilLaxContext();


            string? json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Member? member = JsonSerializer.Deserialize<Member>(json);

            // 檢查資料庫中是否已經有相同的購物車記錄
            var existingCart = db.Cart.FirstOrDefault(c => c.MemberId == member.MemberId && c.ProductId == ProductId);

            if (existingCart != null)
            {
                // 如果已經存在相同的購物車記錄，可以選擇更新數量或是拋出錯誤訊息
                existingCart.CartProductQuantity += cvm.txtCount;
            }
            else
            {
                // 如果資料庫中還不存在相同的購物車記錄，則新增一筆新的購物車記錄
                Cart cart = new Cart
                {
                    MemberId = member.MemberId,
                    ProductId = ProductId,
                    CartProductQuantity = cvm.txtCount
                };

                db.Cart.Add(cart);

            }

            db.SaveChanges();


            //新增購物車div中列表
            //將購物車商品資訊存儲到Session中
            //List<CShoppingCartItem> cartItems = new List<CShoppingCartItem>();
            //CShoppingCartItem cartItem = new CShoppingCartItem
            //{
            //    productId = ProductId,
            //    ProductName = cvm.ProductName,
            //    ProductImg = cvm.ProductImg,
            //    price = cvm.ProductPrice,
            //};
            //cartItems.Add(cartItem);

            //string cartJson = JsonSerializer.Serialize(cartItems);
            //HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, cartJson);

            return RedirectToAction("Details", "Carts", null);

        }

        [HttpPost]
        public async Task<IActionResult> ProductToCartAsync(CAddToCartViewModel cvm)
        {
            ChilLaxContext db = new ChilLaxContext();


            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER); // 抓會員id登入的session
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            Cart cart = new Cart();

            cart.MemberId = member.MemberId;
            cart.ProductId = cvm.ProductId;
            cart.CartProductQuantity = cvm.txtCount;

            _context.Cart.Add(cart);
            _context.SaveChanges();
            await _context.SaveChangesAsync();   //將暫存的異動儲存到資料庫，確保資料庫中的資料與內存中的資料保持同步。


            return RedirectToAction("List");

        }

        // 檢視購物車
        public IActionResult CartView()
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
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

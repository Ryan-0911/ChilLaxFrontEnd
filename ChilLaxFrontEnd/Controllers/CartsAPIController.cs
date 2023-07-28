using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics.Metrics;
using Microsoft.Build.Framework;

namespace ChilLaxFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsAPIController : ControllerBase
    {
        private readonly ChilLaxContext _context;
        public CartsAPIController(ChilLaxContext context)
        {
            _context = context;
        }

        // GET: api/CartsAPI/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<string> Delete(int id)
        {
            if (_context.Cart == null)
                return "刪除失敗";

            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);
            int Mid = member.MemberId;

            Cart cart = await _context.Cart
                .Include(c => c.Member)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.MemberId == Mid && c.ProductId == id);

            if (cart == null)
                return "刪除失敗";

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();

            return "刪除成功";
        }

        // GET: api/CartsAPI/Create/4
        [HttpGet("Create/{id}")]
        public async Task<string> Create(int id)
        {
            if (_context.Cart == null)
                return "新增失敗";
            
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);
            int Mid = member.MemberId;
            int Cartqty = 1;
            List<Cart> thisCart = _context.Cart.Where(c => c.MemberId == Mid).ToList();

            for (int i = 0; i < thisCart.Count; i++)
            {
                if (thisCart[i].ProductId == id) 
                {
                    thisCart[i].CartProductQuantity += Cartqty;
                    _context.Cart.Update(thisCart[i]);
                    _context.SaveChanges();
                    return "已有此商品，數量更新成功";
                }
            }

            Cart cart = new Cart();
            cart.MemberId = Mid;
            cart.ProductId = id;
            cart.CartProductQuantity = Cartqty;

            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();

            return "新增成功";
        }

        //POST: api/CartsAPI/SaveCartSession
        [HttpPost]
        [Route("SaveCartSession")]
        public string SaveCartSession( CartResultReq cartResultReq)
        {
            string cartsJson = JsonSerializer.Serialize(cartResultReq);
            HttpContext.Session.SetString(CDictionary.SK_CHECKOUT_DATA, cartsJson);

            return "list";
        }

        //POST: api/CartsAPI/SaveProductOrder
        [HttpPost]
        [Route("SaveProductOrder")]
        public string SaveProductOrder(ProductOrderReq por)
        {
            string memberjson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            string cartjson = HttpContext.Session.GetString(CDictionary.SK_CHECKOUT_DATA);
            Member member = JsonSerializer.Deserialize<Member>(memberjson);
            CartResultReq CartResultReq = JsonSerializer.Deserialize<CartResultReq>(cartjson);
            int totoPrice = 0;
            for (int i =0; i< CartResultReq.trueCheckboxs.Length; i++)
            {
                int pid = CartResultReq.trueCheckboxs[i].pid;
                var product = _context.Product.Where(p => p.ProductId == pid);
                totoPrice += product.FirstOrDefault().ProductPrice * CartResultReq.trueCheckboxs[i].qty;
            }

            //新增訂單
            ProductOrder productOrder = new ProductOrder();
            productOrder.MemberId = member.MemberId;
            productOrder.OrderPayment = false;
            productOrder.OrderTotalPrice = totoPrice;
            productOrder.OrderDelivery = false;
            productOrder.OrderAddress = member.MemberAddress;
            productOrder.OrderDate = DateTime.Parse(por.OrderDate);
            productOrder.OrderNote = por.OrderNote;
            productOrder.OrderState = "未出貨";
            _context.ProductOrder.Add(productOrder);
            _context.SaveChanges();

            //新增訂單詳細資料表
            int orderid = _context.ProductOrder.Max().OrderId;
            for(int i=0; i< CartResultReq.trueCheckboxs.Length; i++)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderId = orderid;
                orderDetail.ProductId = CartResultReq.trueCheckboxs[i].pid;
                orderDetail.CartProductQuantity = CartResultReq.trueCheckboxs[i].qty;
                _context.OrderDetail.Add(orderDetail);
            }

            return "list";
        }
    }
}

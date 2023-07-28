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
    }
}

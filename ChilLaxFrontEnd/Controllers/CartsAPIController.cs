using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using System.Text.Json;
using ChilLaxFrontEnd.ViewModels;

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


        // 郁霖原本
        // GET: api/CartsAPI/Create/4
        //    [HttpGet("Create/{id}")]
        //    public async Task<string> Create(int id)
        //    {
        //        if (_context.Cart == null)
        //            return "新增失敗";

        //        string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
        //        Console.WriteLine(json);
        //        Member member = JsonSerializer.Deserialize<Member>(json);
        //        int Mid = member.MemberId;
        //        int Cartqty = 1;
        //        List<Cart> thisCart = _context.Cart.Where(c => c.MemberId == Mid).ToList();

        //        for (int i = 0; i < thisCart.Count; i++)
        //        {
        //            if (thisCart[i].ProductId == id) 
        //            {
        //                thisCart[i].CartProductQuantity += Cartqty;
        //                _context.Cart.Update(thisCart[i]);
        //                _context.SaveChanges();
        //                return "已有此商品，數量更新成功";
        //            }
        //        }

        //        Cart cart = new Cart();
        //        cart.MemberId = Mid;
        //        cart.ProductId = id;
        //        cart.CartProductQuantity = Cartqty;

        //        _context.Cart.Add(cart);
        //        await _context.SaveChangesAsync();

        //        return "新增成功";
        //    }
        //}


        // 琬亭修改
        [HttpPost("Create")]
        public async Task<string> Create(CAddToCartViewModel cvm)
        {


            if (_context.Cart == null)
                return "新增失敗";

            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);
            int Mid = member.MemberId;
            int Cartqty = cvm.txtCount;
            List<Cart> thisCart = _context.Cart.Where(c => c.MemberId == Mid).ToList();

            for (int i = 0; i < thisCart.Count; i++)
            {
                if (thisCart[i].ProductId == cvm.ProductId)
                {
                    thisCart[i].CartProductQuantity += Cartqty;
                    _context.Cart.Update(thisCart[i]);
                    _context.SaveChanges();
                    return "已有此商品，數量更新成功";
                }
            }

            Cart cart = new Cart();
            cart.MemberId = Mid;
            cart.ProductId = cvm.ProductId;
            cart.CartProductQuantity = Cartqty;

            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();

            return "新增成功";
        }
    }











    //    // GET: api/CartsAPI
    //    [HttpGet]
    //        public async Task<ActionResult<IEnumerable<Cart>>> GetCart()
    //        {
    //          if (_context.Cart == null)
    //          {
    //              return NotFound();
    //          }
    //            return await _context.Cart.ToListAsync();
    //        }

    //        // GET: api/CartsAPI/5
    //        [HttpGet("{id}")]
    //        public async Task<ActionResult<Cart>> GetCart(int id)
    //        {
    //          if (_context.Cart == null)
    //          {
    //              return NotFound();
    //          }
    //            var cart = await _context.Cart.FindAsync(id);

    //            if (cart == null)
    //            {
    //                return NotFound();
    //            }

    //            return cart;
    //        }

    //        // PUT: api/CartsAPI/5
    //        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //        [HttpPut("{id}")]
    //        public async Task<IActionResult> PutCart(int id, Cart cart)
    //        {
    //            if (id != cart.MemberId)
    //            {
    //                return BadRequest();
    //            }

    //            _context.Entry(cart).State = EntityState.Modified;

    //            try
    //            {
    //                await _context.SaveChangesAsync();
    //            }
    //            catch (DbUpdateConcurrencyException)
    //            {
    //                if (!CartExists(id))
    //                {
    //                    return NotFound();
    //                }
    //                else
    //                {
    //                    throw;
    //                }
    //            }

    //            return NoContent();
    //        }

    //        // POST: api/CartsAPI
    //        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //        [HttpPost]
    //        public async Task<ActionResult<Cart>> PostCart(Cart cart)
    //        {
    //          if (_context.Cart == null)
    //          {
    //              return Problem("Entity set 'ChilLaxContext.Cart'  is null.");
    //          }
    //            _context.Cart.Add(cart);
    //            try
    //            {
    //                await _context.SaveChangesAsync();
    //            }
    //            catch (DbUpdateException)
    //            {
    //                if (CartExists(cart.MemberId))
    //                {
    //                    return Conflict();
    //                }
    //                else
    //                {
    //                    throw;
    //                }
    //            }

    //            return CreatedAtAction("GetCart", new { id = cart.MemberId }, cart);
    //        }

    //        // DELETE: api/CartsAPI/5
    //        [HttpDelete("{id}")]
    //        public async Task<IActionResult> DeleteCart(int id)
    //        {
    //            if (_context.Cart == null)
    //            {
    //                return NotFound();
    //            }
    //            var cart = await _context.Cart.FindAsync(id);
    //            if (cart == null)
    //            {
    //                return NotFound();
    //            }

    //            _context.Cart.Remove(cart);
    //            await _context.SaveChangesAsync();

    //            return NoContent();
    //        }

    //        private bool CartExists(int id)
    //        {
    //            return (_context.Cart?.Any(e => e.MemberId == id)).GetValueOrDefault();
    //        }
    //    }
}

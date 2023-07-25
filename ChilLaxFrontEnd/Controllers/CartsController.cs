using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.Models.DTO;


namespace ChilLaxFrontEnd.Controllers
{
    public class CartsController : Controller
    {
        private readonly ChilLaxContext _context;

        public CartsController(ChilLaxContext context)
        {
            _context = context;
        }

        int mid = 1;

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var chilLaxContext = _context.Cart.Include(c => c.Member).Include(c => c.Product);
            return View(await chilLaxContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<ActionResult<List<CartDTO>>> Details(int? id)
        {
            if (id == null || _context.Cart == null) return NotFound();
   
            var cart = await _context.Cart
                .Include(c => c.Member)
                .Include(c => c.Product)
                .Where(c => c.MemberId == id)
                .Join(_context.Member, c => c.MemberId, m => m.MemberId,(c, m) => new {Carts = c, Members = m})
                .Join(_context.Product, c => c.Carts.ProductId, p => p.ProductId, (c, p) =>new CartDTO {
                    Cart = c.Carts,
                    Member = c.Members,
                    Product = p
                    })
                .ToListAsync();

            if (cart == null) return NotFound();
            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "MemberId");
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,ProductId,CartProductQuantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "MemberId", cart.MemberId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "MemberId", cart.MemberId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,ProductId,CartProductQuantity")] Cart cart)
        {
            if (id != cart.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "MemberId", cart.MemberId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        [HttpDelete("{id}")]
        public async Task<string> Delete(int? id)
        {
            if (id == null || _context.Cart == null) return "刪除失敗";

            Cart? cart = await _context.Cart
                .Include(c => c.Member)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.MemberId == mid && c.ProductId == id);
            if (cart == null) return "刪除失敗";

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();

            return "刪除成功";
        }

        //// POST: Carts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Carts == null)
        //    {
        //        return Problem("Entity set 'ChilLaxContext.Carts'  is null.");
        //    }
        //    var cart = await _context.Carts.FindAsync(id);
        //    if (cart != null)
        //    {
        //        _context.Carts.Remove(cart);
        //    }
            
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool CartExists(int id)
        {
          return (_context.Cart?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
    }
}

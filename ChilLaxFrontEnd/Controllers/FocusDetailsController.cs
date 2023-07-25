using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.Models.DTO;

namespace ChilLaxFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FocusDetailsController : ControllerBase
    {
        private readonly ChilLaxContext _context;

        public FocusDetailsController(ChilLaxContext context)
        {
            _context = context;
        }

        // GET: api/FocusDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FocusDetail>>> GetFocusDetails()
        {
          if (_context.FocusDetail == null)
          {
              return NotFound();
          }
            return await _context.FocusDetail.ToListAsync();
        }

        // GET: api/FocusDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FocusDetail>> GetFocusDetail(string id)
        {
          if (_context.FocusDetail == null)
          {
              return NotFound();
          }
            var focusDetail = await _context.FocusDetail.FindAsync(id);

            if (focusDetail == null)
            {
                return NotFound();
            }

            return focusDetail;
        }

        // PUT: api/FocusDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFocusDetail(string id, FocusDetail focusDetail)
        {
            if (id != focusDetail.FocusDetailId)
            {
                return BadRequest();
            }

            _context.Entry(focusDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FocusDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FocusDetails
        // 新增資料至 FocusDetail 與 PointHistory 資料表
        [HttpPost]
        public async Task<string> PostFocusDetail([FromBody] FocusDetailWithPointHistoryDTO data)
        {
          if (_context.FocusDetail == null || _context.PointHistory == null)
          {
              return "領取失敗";
          }
            _context.FocusDetail.Add(data.FocusDetail);
            _context.PointHistory.Add(data.PointHistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FocusDetailExists(data.FocusDetail.FocusDetailId) || PointHistoryExists(data.PointHistory.PointDetailId))
                {
                    return "領取失敗";
                }
                else
                {
                    throw;
                }
            }
            return "領取成功";
        }

        // DELETE: api/FocusDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFocusDetail(string id)
        {
            if (_context.FocusDetail == null)
            {
                return NotFound();
            }
            var focusDetail = await _context.FocusDetail.FindAsync(id);
            if (focusDetail == null)
            {
                return NotFound();
            }

            _context.FocusDetail.Remove(focusDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FocusDetailExists(string id)
        {
            return (_context.FocusDetail?.Any(e => e.FocusDetailId == id)).GetValueOrDefault();
        }

        private bool PointHistoryExists(string id)
        {
            return (_context.PointHistory?.Any(e => e.PointDetailId == id)).GetValueOrDefault();
        }
    }
}

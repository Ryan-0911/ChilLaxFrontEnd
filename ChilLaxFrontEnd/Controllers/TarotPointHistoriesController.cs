using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using System.Text.Json;
using ChilLaxFrontEnd.Models.DTO;

namespace ChilLaxFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarotPointHistoriesController : ControllerBase
    {
        private readonly ChilLaxContext _context;

        public TarotPointHistoriesController(ChilLaxContext context)
        {
            _context = context;
        }

        // GET: api/TarotPointHistories
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<PointHistory>>> GetPointHistory()
        //{
        //    if (_context.CustomerService == null)
        //    {
        //        return NotFound();
        //    }
        //    string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
        //    //Console.WriteLine(json);
        //    Member member = JsonSerializer.Deserialize<Member>(json);

        //    return await _context.PointHistory.Where(Cs => Cs.MemberId == member.MemberId).ToListAsync();
        //}
        public async Task<ActionResult<decimal>> GetTotalModifiedAmount()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Member member = JsonSerializer.Deserialize<Member>(json);

            // 取得指定 MemberId 的 ModifiedAmount 欄位值加總
            decimal totalModifiedAmount = await _context.PointHistory
                .Where(ph => ph.MemberId == member.MemberId)
                .SumAsync(ph => ph.ModifiedAmount);

            return totalModifiedAmount;
        }



        // POST: api/TarotPointHistories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        
        public async Task<TarotPointHistoryDTO> PostPointHistory(TarotPointHistoryDTO TarotPointHistoryDTO)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            //Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);
            PointHistory pot = new PointHistory
            {
                PointDetailId = TarotPointHistoryDTO.PointDetailId,
                MemberId = member.MemberId,
                ModifiedSource = TarotPointHistoryDTO.ModifiedSource,
                ModifiedAmount = TarotPointHistoryDTO.ModifiedAmount,

            };
            _context.PointHistory.Add(pot);
            await _context.SaveChangesAsync();

            return TarotPointHistoryDTO;
        }

        // DELETE: api/TarotPointHistories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePointHistory(string id)
        {
            if (_context.PointHistory == null)
            {
                return NotFound();
            }
            var pointHistory = await _context.PointHistory.FindAsync(id);
            if (pointHistory == null)
            {
                return NotFound();
            }

            _context.PointHistory.Remove(pointHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PointHistoryExists(string id)
        {
            return (_context.PointHistory?.Any(e => e.PointDetailId == id)).GetValueOrDefault();
        }
    }
}

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

        // POST: api/FocusDetails
        // 新增資料至 FocusDetail 與 PointHistory 資料表
        [HttpPost]
        public async Task<string> PostFocusAndPoint([FromBody] FocusDetailWithPointHistoryDTO data)
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


        [HttpGet("{memberId}")]
        public async Task<Boolean> SearchFocusToday(int memberId)
        {
            // 取得今天的日期
            DateTime today = DateTime.Today;

            // 查詢該會員今天的專注時間總和
            int totalDurationToday = _context.PointHistories
                .Where(p => p.MemberId == memberId && p.ModifiedSource == "Focus")
                .Join(_context.FocusDetails,
                    pointHistory => pointHistory.PointDetailId,
                    focusDetail => focusDetail.FocusDetailId,
                    (pointHistory, focusDetail) => new { PointHistory = pointHistory, FocusDetail = focusDetail })
                .Where(joined => joined.FocusDetail.StartDatetime.Date == today)
                .Sum(joined => joined.FocusDetail.Duration);

            // 檢查專注時間是否大於 60 分鐘
            bool isFocusGreaterThan60 = totalDurationToday >= 60;

            return isFocusGreaterThan60;
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

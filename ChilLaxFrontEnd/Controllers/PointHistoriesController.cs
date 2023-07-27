using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChilLaxFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointHistoriesController : ControllerBase
    {
        private readonly ChilLaxContext _context;

        public PointHistoriesController(ChilLaxContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IEnumerable<PointRecordDTO>> GetPointFocus()
        {
            // 從 session 取出會員 ID
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            // Focus
            var queryF = from pointHistory in _context.PointHistory
                         join focusDetail in _context.FocusDetail
                         on pointHistory.PointDetailId equals focusDetail.FocusDetailId
                         where pointHistory.MemberId == member.MemberId
                         select new PointRecordDTO
                         {
                             ModifiedSource = pointHistory.ModifiedSource,
                             ModifiedAmount = pointHistory.ModifiedAmount,
                             Content = $"專注時間: {(focusDetail.Duration).ToString()}分",
                             ModifiedTime = focusDetail.EndDatetime,
                         };

            return queryF.OrderByDescending(f => f.ModifiedAmount);
        }


        [HttpPost]
        public async Task<IEnumerable<PointRecordDTO>> GetPointRecords(string? keyword, DateTime? startDate, DateTime? endDate, string? sortBy, string? sortType)
        {
            //string? keyword = searchDTO.keyword; //下拉式選單- Focus、Tarot、Product
            //int? page = searchDTO.page ?? 1;
            //string? sortBy = searchDTO.sortBy;
            //string? sortType = searchDTO.sortType ?? "asc";

            // 從 session 取出會員 ID
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            // Focus
            var queryF = from pointHistory in _context.PointHistory
                         join focusDetail in _context.FocusDetail
                         on pointHistory.PointDetailId equals focusDetail.FocusDetailId
                         where pointHistory.MemberId == member.MemberId
                         select new PointRecordDTO
                         {
                             ModifiedSource = pointHistory.ModifiedSource,
                             ModifiedAmount = pointHistory.ModifiedAmount,
                             Content = $"專注時間: {(focusDetail.Duration).ToString()}分",
                             ModifiedTime = focusDetail.EndDatetime,
                         };

            // 日期
            if (startDate != null)
            {
                queryF = queryF.Where(f => f.ModifiedTime >= startDate);
            }
            if (endDate != null)
            {
                queryF = queryF.Where(f => f.ModifiedTime <= endDate);
            }

            Console.WriteLine(sortBy);
            //排序
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy)
                {
                    case "modifiedAmount":
                        queryF = sortType == "asc" ? queryF.OrderBy(f => f.ModifiedAmount) : queryF.OrderByDescending(f => f.ModifiedAmount);
                        break;
                    case "modifiedTime":
                        queryF = sortType == "asc" ? queryF.OrderBy(f => f.ModifiedTime) : queryF.OrderByDescending(f => f.ModifiedTime);
                        break;
                    default:
                        return queryF;
                }
            }



            // Tarot
            var queryT = from pointHistory in _context.PointHistory
                         join TarotOrder in _context.TarotOrder
                         on pointHistory.PointDetailId equals TarotOrder.TarotOrderId
                         where pointHistory.MemberId == member.MemberId
                         select new PointRecordDTO
                         {
                             ModifiedSource = pointHistory.ModifiedSource,
                             ModifiedAmount = pointHistory.ModifiedAmount,
                             Content = $"抽牌結果: {(TarotOrder.CardResult).ToString()}",
                             ModifiedTime = TarotOrder.TarotTime,
                         };

            // 日期
            if (startDate != null)
            {
                queryT = queryT.Where(t => t.ModifiedTime >= startDate);
            }
            if (endDate != null)
            {
                queryT = queryT.Where(t => t.ModifiedTime <= endDate);
            }

            // 排序
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy)
                {
                    case "modifiedAmount":
                        queryT = sortType == "asc" ? queryT.OrderBy(t => t.ModifiedAmount) : queryT.OrderByDescending(t => t.ModifiedAmount);
                        break;
                    case "modifiedTime":
                        queryT = sortType == "asc" ? queryT.OrderBy(t => t.ModifiedTime) : queryT.OrderByDescending(t => t.ModifiedTime);
                        break;
                    default:
                        return queryT;
                }
            }

            //// Product
            //var queryP = from pointHistory in _context.PointHistory.ToList()
            //            join productOrder in _context.ProductOrder.ToList()
            //            on pointHistory.PointDetailId equals productOrder.OrderId.ToString()
            //            where pointHistory.MemberId == member.MemberId
            //            select new PointRecordDTO
            //            {
            //                ModifiedSource = pointHistory.ModifiedSource,
            //                ModifiedAmount = pointHistory.ModifiedAmount,
            //                Content = $"消費總額: {(productOrder.OrderTotalPrice).ToString()}",
            //                ModifiedTime = productOrder.OrderDate,
            //            };

            //Console.WriteLine(queryP);



            // 將上面三個 PointRecordDTO 加在同一個 PointRecordDTO
            // 從資料庫中分別獲取結果
            var resultF = await queryF.ToListAsync();
            var resultT = await queryT.ToListAsync();

            switch (keyword)
            {
                case "All":
                    return resultF.Concat(resultT).ToList();
                    break;
                case "Focus":
                    return queryF.ToList();
                    break;
                case "Tarot":
                    return queryT.ToList();
                    break;
                //case "Product":
                //    return queryP.ToList();
                //    break;
                default:
                    return resultF.Concat(resultT).ToList();
            }
        }
    }
}


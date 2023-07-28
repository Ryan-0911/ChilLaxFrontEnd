using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChilLaxFrontEnd.Controllers
{
    // api/PointHisories
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
        public async Task<IEnumerable<PointRecordDTO>> GetPointProduct()
        {
            // 從 session 取出會員 ID
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            // Product
            var queryP = from pointHistory in _context.PointHistory
                         join productOrder in _context.ProductOrder
                         on pointHistory.PointDetailId equals productOrder.OrderId.ToString()
                         where pointHistory.MemberId == member.MemberId
                         select new PointRecordDTO
                         {
                             ModifiedSource = pointHistory.ModifiedSource,
                             ModifiedAmount = pointHistory.ModifiedAmount,
                             Content = $"消費總額: {(productOrder.OrderTotalPrice).ToString()}",
                             ModifiedTime = productOrder.OrderDate,
                         };

            return queryP;
        }


        [HttpPost]
        public async Task<PointRecordsPagingDTO> GetPointRecords(string? keyword, DateTime? startDate, DateTime? endDate, string? sortBy, string? sortType, int? page = 1)
        {
            // 從 session 取出會員 ID
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            int pageSize = 5; // 每頁顯示的筆數
            int TotalCount = 0; // 共有多少筆資料
            int TotalPages = 0; // 共有幾頁
            List<PointRecordDTO> pointRecords; // 篩選後的點數資料
            PointRecordsPagingDTO prpd = new PointRecordsPagingDTO(); // 最終回傳的結果 (總頁數+點數資料)

            // Focus---------------------------------------------------------------------------------------------------------------------------------------------
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

            // Tarot---------------------------------------------------------------------------------------------------------------------------------------------
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

            //// Product---------------------------------------------------------------------------------------------------------------------------------------------
            var queryP = from pointHistory in _context.PointHistory
                         join productOrder in _context.ProductOrder
                         on pointHistory.PointDetailId equals productOrder.OrderId.ToString()
                         where pointHistory.MemberId == member.MemberId
                         select new PointRecordDTO
                         {
                             ModifiedSource = pointHistory.ModifiedSource,
                             ModifiedAmount = pointHistory.ModifiedAmount,
                             Content = $"消費總額: {(productOrder.OrderTotalPrice).ToString()}",
                             ModifiedTime = productOrder.OrderDate,
                         };

            // 將上面三個 PointRecordDTO 加在同一個 PointRecordDTO *先以ToList()把queryF跟queryT載入記憶體才能操作
            var resultF = await queryF.ToListAsync();
            var resultT = await queryT.ToListAsync();
            var resultP = await queryP.ToListAsync();
            //var resultP = await queryP.ToListAsync();
            var resultAll = resultF.Concat(resultT).Concat(resultP).ToList();


            // 根據下拉選單的選擇進行組合
            switch (keyword)
            {
                case "All":
                    // 日期
                    if (startDate != null)
                    {
                        resultAll = resultAll.Where(all => all.ModifiedTime >= startDate).ToList();
                    }
                    if (endDate != null)
                    {
                        resultAll = resultAll.Where(all => all.ModifiedTime <= endDate).ToList();
                    }
                    // 排序
                    if (!string.IsNullOrWhiteSpace(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "modifiedAmount":
                                resultAll = sortType == "asc" ? resultAll.OrderBy(all => all.ModifiedAmount).ToList() : resultAll.OrderByDescending(all => all.ModifiedAmount).ToList();
                                break;
                            case "modifiedTime":
                                resultAll = sortType == "asc" ? resultAll.OrderBy(all => all.ModifiedTime).ToList() : resultAll.OrderByDescending(all => all.ModifiedTime).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    // 分頁
                    TotalCount = resultAll.Count(); // 共有多少筆資料
                    TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);  // 共有幾頁
                    pointRecords = resultAll.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();
                    prpd.TotalPages = TotalPages;
                    prpd.PointRecords = pointRecords;
                    return prpd;
                    break;

                case "Focus":
                    // 日期
                    if (startDate != null)
                    {
                        queryF = queryF.Where(f => f.ModifiedTime >= startDate);
                    }
                    if (endDate != null)
                    {
                        queryF = queryF.Where(f => f.ModifiedTime <= endDate);
                    }
                    // 排序
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
                                break;
                        }
                    }
                    // 分頁
                    TotalCount = queryF.Count(); // 共有多少筆資料
                    TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);  // 共有幾頁
                    pointRecords = queryF.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();

                    prpd.TotalPages = TotalPages;
                    prpd.PointRecords = pointRecords;
                    return prpd;
                    break;

                case "Tarot":
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
                                break;
                        }
                    }
                    // 分頁
                    TotalCount = queryT.Count(); // 共有多少筆資料
                    TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);  // 共有幾頁
                    pointRecords = queryT.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();

                    prpd.TotalPages = TotalPages;
                    prpd.PointRecords = pointRecords;
                    return prpd;
                    break;

                case "Product":
                    // 日期
                    if (startDate != null)
                    {
                        queryP = queryP.Where(p => p.ModifiedTime >= startDate);
                    }
                    if (endDate != null)
                    {
                        queryP = queryP.Where(p => p.ModifiedTime <= endDate);
                    }
                    // 排序
                    if (!string.IsNullOrWhiteSpace(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "modifiedAmount":
                                queryP = sortType == "asc" ? queryP.OrderBy(p => p.ModifiedAmount) : queryP.OrderByDescending(p => p.ModifiedAmount);
                                break;
                            case "modifiedTime":
                                queryP = sortType == "asc" ? queryP.OrderBy(p => p.ModifiedTime) : queryP.OrderByDescending(p => p.ModifiedTime);
                                break;
                            default:
                                break;
                        }
                    }
                    // 分頁
                    TotalCount = queryP.Count(); // 共有多少筆資料
                    TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);  // 共有幾頁
                    pointRecords = queryP.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();

                    prpd.TotalPages = TotalPages;
                    prpd.PointRecords = pointRecords;
                    return prpd;
                    break;

                    return prpd;

                default:
                    // 日期
                    if (startDate != null)
                    {
                        resultAll = resultAll.Where(all => all.ModifiedTime >= startDate).ToList();
                    }
                    if (endDate != null)
                    {
                        resultAll = resultAll.Where(all => all.ModifiedTime <= endDate).ToList();
                    }
                    // 排序
                    if (!string.IsNullOrWhiteSpace(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "modifiedAmount":
                                resultAll = sortType == "asc" ? resultAll.OrderBy(all => all.ModifiedAmount).ToList() : resultAll.OrderByDescending(all => all.ModifiedAmount).ToList();
                                break;
                            case "modifiedTime":
                                resultAll = sortType == "asc" ? resultAll.OrderBy(all => all.ModifiedTime).ToList() : resultAll.OrderByDescending(all => all.ModifiedTime).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    // 分頁
                    TotalCount = resultAll.Count(); // 共有多少筆資料
                    TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);  // 共有幾頁
                    pointRecords = resultAll.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();
                    prpd.TotalPages = TotalPages;
                    prpd.PointRecords = pointRecords;
                    return prpd;
                    break;
            }
        }
    }
}
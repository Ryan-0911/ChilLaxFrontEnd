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

        // GET: api/PointHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.PointHistory>>> GetPointHistories()
        {
            if (_context.PointHistories == null)
            {
                return NotFound();
            }
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            return await _context.PointHistories.Where(ph => ph.MemberId == member.MemberId).ToListAsync();
        }

        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<PointRecordDTO>> GetPointRecords([FromBody] SearchDTO searchDTO) // string? keyword, string? sortBy, string? sortType, int page = 1
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            string? keyword = searchDTO.keyword; //下拉式選單- Focus、Tarot、Product
            int? page = searchDTO.page ?? 1;
            string? sortBy = searchDTO.sortBy;
            string? sortType = searchDTO.sortType ?? "asc";

            // FocusDetail + PointHistory = PointRecordDTO
            var query = from pointHistory in _context.PointHistories
                        join focusDetail in _context.FocusDetails
                        on pointHistory.PointDetailId equals focusDetail.FocusDetailId
                        where pointHistory.MemberId == member.MemberId
                            select new PointRecordDTO
                        {
    
                        };

            // TarotOrder + PointHistory = PointRecordDTO
            // ProductOrder + PointHistory = PointRecordDTO



            // 將上面三個 PointRecordDTO 加在同一個 PointRecordDTO






            //var pointRecord = _context.PointHistories.AsQueryable();

            ////下拉式選單搜尋
            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    pointRecord = _context.PointHistories.Where(ph => ph.ModifiedSource.Contains(keyword));
            //}

            ////排序
            //switch (sortBy)
            //{
            //    case "productName":
            //        products = sortType == "asc" ? products.OrderBy(p => p.ProductName) : products.OrderByDescending(p => p.ProductName);
            //        break;
            //    case "unitPrice":
            //        products = sortType == "asc" ? products.OrderBy(p => p.UnitPrice) : products.OrderByDescending(p => p.UnitPrice);
            //        break;
            //    case "unitsInStock":
            //        products = sortType == "asc" ? products.OrderBy(p => p.UnitsInStock) : products.OrderByDescending(p => p.UnitsInStock);
            //        break;
            //    default:
            //        products = sortType == "asc" ? products.OrderBy(p => p.ProductId) : products.OrderByDescending(p => p.ProductId);
            //        break;
            //}

            //int TotalCount = products.Count(); //總共有多少筆資料
            //int TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);  //計算總共有幾頁

            //分頁
            //page = 1, Skip(0)
            //     page = 2, Skip(10)
            //     page = 3 , Skie(20)
            //     var pageProducts = await products.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToListAsync();

            //ProductsPagingDTO prodDTO = new ProductsPagingDTO();
            //prodDTO.TotalPages = TotalPages;
            //prodDTO.ProductsResult = pageProducts;

            //return prodDTO;
        }
    }
}
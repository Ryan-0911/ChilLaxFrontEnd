using ChilLaxFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChilLaxFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionData : ControllerBase
    {
        private readonly ChilLaxContext _context;

        public SessionData(ChilLaxContext context)
        {
            _context = context;
        }

        // 傳入session 的 key 取得 session 的值
        [HttpGet("{key}")]
        public IActionResult GetUser(string key)
        {
            // 使用 HttpContext.Session 來存取 Session 資料
            var json = HttpContext.Session.GetString(key);
            return Ok(json);
        }

        // 傳入異動數量，更新 SK_LOINGED_USER 的會員點數
        [HttpPost("UpdatePoint/{num}")]
        public IActionResult UpdatePoint(int num)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);
            member.MemberPoint += num;

            // 在交易中進行資料庫更新操作
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 更新會員資料
                    _context.Entry(member).State = EntityState.Modified;
                    _context.SaveChanges();

                    // 提交交易
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // 發生錯誤時回滾交易
                    transaction.Rollback();
                    return BadRequest("更新失敗：" + ex.Message);
                }
            }

            json = JsonSerializer.Serialize(member);
            HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
            return Ok("更新成功");
        }
    }
}

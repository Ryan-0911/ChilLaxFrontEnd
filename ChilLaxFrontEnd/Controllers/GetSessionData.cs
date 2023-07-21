using ChilLaxFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChilLaxFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetSessionData : ControllerBase
    {
        // 在 Controller 中建立 API 路由，處理 GET 請求並返回 Session 資料
        [HttpGet("{key}")]
        public IActionResult GetUser(string key)
        {
            // 使用 HttpContext.Session 來存取 Session 資料
            var session = HttpContext.Session.GetString(key);

            // 在此處理 sessionData 並返回結果
            // 可能需要將資料轉換為 JSON 格式並返回
            return Ok(session);
        }
    }
}

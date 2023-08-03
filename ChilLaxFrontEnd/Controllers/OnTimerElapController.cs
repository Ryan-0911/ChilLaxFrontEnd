//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ChilLaxFrontEnd.Models;
//using System.Net.Mail;
//using System.Net;
//using System.Timers;
//using Timer = System.Timers.Timer;

//namespace ChilLaxFrontEnd.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OnTimerElapController : ControllerBase
//    {
//        private readonly ChilLaxContext _context;

//        private Timer _timer;
//        private int orderCount;
//        private decimal totalSales;
//        private int memberCount;
//        private int focousOrderCount;
//        private int focousMemberCount;

//        public OnTimerElapController(ChilLaxContext context)
//        {
//            _context = context;
//            // 初始化計時器，設定間隔時間為 24 小時 (86400000 毫秒)
//            _timer = new Timer(86400000);
//            // 設定計時器觸發事件
//            _timer.Elapsed += OnTimerElapsed;
//            // 啟動計時器
//            _timer.Start();
//        }

//        public void GetProductOrderReport()
//        {
//            DateTime yesterday = DateTime.Today.AddDays(-1);
//            DateTime today = DateTime.Today;

//            var query = from po in _context.ProductOrder
//                        join od in _context.OrderDetail on po.OrderId equals od.OrderId
//                        where po.OrderDate >= yesterday && po.OrderDate < today && po.OrderPayment == true
//                        select new
//                        {
//                            OrderCount = 1,
//                            TotalSales = po.OrderTotalPrice,
//                            MemberCount = po.MemberId,
//                            ProductCount = od.ProductId,
//                            ProductQuantity = od.CartProductQuantity
//                        };

//            var result = query.GroupBy(x => 1) // Group by a constant to calculate aggregates across all rows
//                              .Select(g => new
//                              {
//                                  OrderCount = g.Sum(x => x.OrderCount),
//                                  TotalSales = g.Sum(x => x.TotalSales),
//                                  MemberCount = g.Select(x => x.MemberCount).Distinct().Count(),
//                                  ProductCount = g.Select(x => x.ProductCount).Distinct().Count(),
//                                  ProductQuantity = g.Sum(x => x.ProductQuantity)
//                              })
//                              .FirstOrDefault();

//            int orderCount = result?.OrderCount ?? 0;
//            decimal totalSales = result?.TotalSales ?? 0;
//            int memberCount = result?.MemberCount ?? 0;
//            int productCount = result?.ProductCount ?? 0;
//            int productQuantity = result?.ProductQuantity ?? 0;
//        }

//        public void GetFocousOrderReport()
//        {
//            DateTime yesterday = DateTime.Today.AddDays(-1);
//            DateTime today = DateTime.Today;

//            var yesterdayFocouseIds = _context.FocusDetail
//                .Where(fd => fd.EndDatetime >= yesterday && fd.EndDatetime < today)
//                .Select(fd => fd.FocusDetailId)
//                .ToList();

//            var result = (from ph in _context.PointHistory
//                          where yesterdayFocouseIds.Contains(ph.PointDetailId)
//                          group ph by 1 into g
//                          select new
//                          {
//                              OrderCount = g.Count(),
//                              MemberCount = g.Select(x => x.MemberId).Distinct().Count()
//                          })
//                         .FirstOrDefault();

//            int orderCount = result?.OrderCount ?? 0;
//            int memberCount = result?.MemberCount ?? 0;

//        }




//        // 定義計時器觸發的事件處理方法
//        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
//        {
//            // 在這裡呼叫您的查詢方法，並將結果放入郵件內容
//            string emailBody = GenerateEmailBody();
//            // 呼叫發送郵件方法
//            SendEmail(emailBody).GetAwaiter().GetResult();
//        }

//        // 定義產生郵件內容的方法
//        private string GenerateEmailBody()
//        {
//            // 執行您的查詢並取得結果
//            GetProductOrderReport();
//            GetFocousOrderReport();

//            // 將查詢結果整理成郵件內容的格式
//            string emailBody = $"今天{DateTime.Now:yyyy/MM/dd}有{orderCount}筆訂單，總銷售金額共{totalSales}，共新增{memberCount}位購買會員。\n" +
//                $"昨天有{focousOrderCount}筆專注訂單，共{focousMemberCount}位專注會員。\n";

//            return emailBody;
//        }

//        private static async Task SendEmail(string emailBody)
//        {
//            var smtpClient = new SmtpClient("smtp.gmail.com")
//            {
//                Port = 587,
//                Credentials = new NetworkCredential("your-email@gmail.com", "your-email-password"),
//                EnableSsl = true,
//            };

//            var message = new MailMessage
//            {
//                From = new MailAddress("your-email@gmail.com"),
//                Subject = "每日報表", // Email 主旨
//                Body = emailBody, // Email 內容，這裡放您的 SQL 查詢結果
//            };

//            message.To.Add(new MailAddress("tuv156123@gmail.com")); // 目標 Email 地址

//            try
//            {
//                await smtpClient.SendMailAsync(message);
//                Console.WriteLine("Email sent successfully!");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Failed to send email: " + ex.Message);
//            }
//        }
        

//    }
//}

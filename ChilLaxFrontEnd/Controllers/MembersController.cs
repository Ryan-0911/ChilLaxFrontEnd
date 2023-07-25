using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.ViewModels;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics.Metrics;
using Google;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient.Server;
using Microsoft.AspNetCore.Cors;



namespace ChilLaxFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ChilLaxContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MembersController(ChilLaxContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        ChilLaxContext db = new ChilLaxContext();
        //public async Task<IActionResult> PutMember(int id, Member member)
        // if (id != member.MemberId)
        //    {
        //        return BadRequest();
        //    }
        //    _context.Entry(member).State = EntityState.Modified;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MemberExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();


        //[HttpPatch("{id}")]
        // PUT: api/Members/5
        [HttpPut("{id}")]
        public async Task<string> PutMember(int id, VerifyEmailViewModel VE)
        {   //public async Task<string> SaveData([FromBody] VerifyEmailViewModel VE)
            if (id != VE.MemberId)
            {
                return "驗證失敗";

            }
            Member Mem = await _context.Member.FindAsync(VE.MemberId);
            if (Mem != null)
            {
                Mem.MemberId = VE.MemberId;
                Mem.MemberEmail = VE.MemberEmail;
                Mem.IsValid = false;

                _context.Entry(Mem).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    await BuildEmailTemplate(VE.MemberId);
                    
                    return "已發送驗證信，請前往Email查看!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(id))
                    {
                        return "系統忙碌中，請稍後再試!";
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            return "驗證失敗!";
        }

        public async Task BuildEmailTemplate(int regID)
        {
            string body;
            try {
                body = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplate", "Text.cshtml"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            //string body = $@"
            //            <html>
            //            <head>
            //                <title>驗證帳號</title>
            //            </head>
            //            <body>
            //                <h1>歡迎您，@ViewBag.UserName</h1>
            //                <p>感謝您註冊成為我們的會員。</p>
            //                <p>請點擊以下連結來驗證您的帳號：</p>
            //                <a href=""https://localhost:7189/api/Members/Verify/{regID}"">點擊這裡</a>
            //            </body>
            //            </html>
            //        ";
            var regInfo = db.Member.Where(x => x.MemberId == regID).FirstOrDefault();
            var url = "https://localhost:7189/api/Members/Verify/" + regID;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            string UserName = regInfo.MemberName;
            body = body.Replace("@ViewBag.UserName", UserName);
            body = body.Replace("@ViewBag.regID", regID.ToString());

            body = body.ToString();

            await BuildEmailAsync("驗證帳號", body, regInfo.MemberEmail);
        }

        public static async Task BuildEmailAsync(string subjectText, string bodyText, string sendTo)
        {
            string GoogleID = "chillax20230808@gmail.com"; //Google 發信帳號
            string TempPwd = "gzwmfcbpepypgikf"; //應用程式密碼

            string to, body, bcc, cc;
            to = sendTo.Trim();
            bcc = "";
            cc = "";

            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(GoogleID);//發件人
            mail.To.Add(new MailAddress(to));//收件人
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subjectText;//郵件主題
            mail.Body = body;//郵件內文
            mail.IsBodyHtml = true;
            mail.SubjectEncoding = Encoding.UTF8;

            string SmtpServer = "smtp.gmail.com";
            int SmtpPort = 587;
            using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))//使用郵件伺服器來發送這個郵件
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(GoogleID, TempPwd);//寄信帳密 
                try
                {
                    await client.SendMailAsync(mail);//寄出信件，使用SendMailAsync進行非同步寄信
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        [HttpPut("Verify/{regID}")]
        public async Task<IActionResult> Verify(int regID)
        {
            var user = db.Member.FirstOrDefault(x => x.MemberId == regID);
            if (user != null)
            {
                user.IsValid = true;
                await db.SaveChangesAsync();

                var response = new 
                { 
                    message = "驗證成功，請登入！" ,
                    closePageUrl = "https://localhost:7189/Login/verifyEmail"
                };
                return Ok(response);
                //return RedirectToAction("Login", "Login");
            }

            var errorResponse = new { message = "驗證失敗！" };
            return NotFound(errorResponse);
        }





        // GET: api/Members
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        //{
        //  if (_context.Members == null)
        //  {
        //      return NotFound();
        //  }
        //    return await _context.Members.ToListAsync();
        //}

        //// GET: api/Members/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Member>> GetMember(int id)
        //{
        //  if (_context.Members == null)
        //  {
        //      return NotFound();
        //  }
        //    var member = await _context.Members.FindAsync(id);

        //    if (member == null)
        //    {
        //        return NotFound();
        //    }

        //    return member;
        //}


        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMember(int id, Member member)
        //{
        //    if (id != member.MemberId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(member).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MemberExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Member>> PostMember(Member member)
        //{
        //  if (_context.Members == null)
        //  {
        //      return Problem("Entity set 'ChilLaxContext.Members'  is null.");
        //  }
        //    _context.Members.Add(member);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetMember", new { id = member.MemberId }, member);
        //}

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            if (_context.Member == null)
            {
                return NotFound();
            }
            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Member.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return (_context.Member?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
    }
}

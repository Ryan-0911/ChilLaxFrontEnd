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
    public class CustomerServicesApiController : ControllerBase
    {
        private readonly ChilLaxContext _context;

        public CustomerServicesApiController(ChilLaxContext context)
        {
            _context = context;
        }

        // GET: api/CustomerServicesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerService>>> GetCustomerService()
        {
            if (_context.CustomerService == null)
            {
                return NotFound();
            }
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            //Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);

            return await _context.CustomerService.Where(Cs => Cs.MemberId == member.MemberId).ToListAsync();


            
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerService(int id, CustomerService customerService)
        {
            if (id != customerService.CustomerServiceId)
            {
                return BadRequest();
            }

            _context.Entry(customerService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerServiceExists(id))
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

        // POST: api/CustomerServicesApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<CustomerServiceDTO> PostCustomerService(CustomerServiceDTO cusDTO)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            //Console.WriteLine(json);
            Member member = JsonSerializer.Deserialize<Member>(json);
            CustomerService cus = new CustomerService
          { 
            MemberId = member.MemberId,
            MessageDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Message = cusDTO.Message,
          
          };
            _context.CustomerService.Add(cus);
            await _context.SaveChangesAsync();

            return cusDTO;
        }

        // DELETE: api/CustomerServicesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerService(int id)
        {
            if (_context.CustomerService == null)
            {
                return NotFound();
            }
            var customerService = await _context.CustomerService.FindAsync(id);
            if (customerService == null)
            {
                return NotFound();
            }

            _context.CustomerService.Remove(customerService);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerServiceExists(int id)
        {
            return (_context.CustomerService?.Any(e => e.CustomerServiceId == id)).GetValueOrDefault();
        }
    }
}

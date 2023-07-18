using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChilLaxFrontEnd.Models;
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
        public async Task<ActionResult<IEnumerable<PointHistory>>> GetPointHistories()
        {
            if (_context.PointHistories == null)
            {
                return NotFound();
            }
            return await _context.PointHistories.ToListAsync();
        }

    }
}

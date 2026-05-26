using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaServerApp.Models.Data; 

namespace MediaServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly MediaContext _context;

        public ChartsController(MediaContext context)
        {
            _context = context;
        }

        [HttpGet("countByYear")]
        public async Task<JsonResult> GetCountByYear()
        {
            var data = await _context.Movies
                .Where(m => m.ReleaseDate != null)
                .GroupBy(m => m.ReleaseDate.Value.Year)
                .Select(g => new { Year = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            return new JsonResult(data);
        }

        [HttpGet("countByGenre")]
        public async Task<JsonResult> GetCountByGenre()
        {
            var data = await _context.Genres
                .Select(g => new { Genre = g.Name, Count = g.Movies.Count })
                .ToListAsync();

            return new JsonResult(data);
        }
    }
}
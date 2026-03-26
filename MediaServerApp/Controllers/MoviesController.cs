using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaServerApp.Models.Data;

namespace MediaServerApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MediaContext _context;

        public MoviesController(MediaContext context)
        {
            _context = context;
        }

        // get library
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        // get details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Genres) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            return View(movie);
        }
    }
}
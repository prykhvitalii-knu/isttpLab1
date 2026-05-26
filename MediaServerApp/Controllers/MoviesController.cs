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

        public async Task<IActionResult> Index(string searchString, int? genreId)
        {
            var moviesQuery = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                moviesQuery = moviesQuery.Where(m => m.Title.ToLower().Contains(searchString.ToLower()));
            }

            if (genreId.HasValue) {
                // Переконайся, що назва колекції (Genres чи MovieGenres) збігається з твоєю моделлю Movie
                moviesQuery = moviesQuery.Where(m => m.Genres.Any(g => g.Id == genreId));
                
                var selectedGenre = await _context.Genres.FindAsync(genreId);
                ViewBag.PageTitle = selectedGenre?.Name;
            }
            else {
                ViewBag.PageTitle = "Home";
            }

            ViewBag.SavedMovieIds = await _context.SavedItems.Select(s => s.MovieId).ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            ViewBag.CurrentGenre = genreId;
            ViewBag.CurrentSearch = searchString;

            return View(await moviesQuery.ToListAsync());
        }



        public async Task<IActionResult> Saved()
        {
            var savedMovies = await _context.SavedItems
                .Include(s => s.Movie)
                .Select(s => s.Movie)
                .ToListAsync();

            ViewBag.PageTitle = "Saved Movies"; 
            ViewBag.SavedMovieIds = savedMovies.Select(m => m.Id).ToList();

            return View("Index", savedMovies); 
        }


        [HttpPost]
        public async Task<IActionResult> ToggleSave(int movieId)
        {
            var existing = await _context.SavedItems.FirstOrDefaultAsync(s => s.MovieId == movieId);
            if (existing != null) {
                _context.SavedItems.Remove(existing);
            } else {
                _context.SavedItems.Add(new SavedItem { MovieId = movieId, UserId = 1 }); 
            }
            await _context.SaveChangesAsync();
            string returnUrl = Request.Headers["Referer"].ToString();

            // Якщо адреса є — повертаємо на неї, якщо ні — на головну
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            var historyEntry = await _context.WatchHistories
                .FirstOrDefaultAsync(h => h.UserId == 1 && h.MovieId == movie.Id);

            if (historyEntry == null)
            {

                historyEntry = new WatchHistory
                {
                    UserId = 1,
                    MovieId = movie.Id,
                    LastWatched = DateTime.UtcNow,
                    TimeStop = 0 // Поки 0, бо тільки почали дивитися
                };
                _context.WatchHistories.Add(historyEntry);
            }
            else
            {
                historyEntry.LastWatched = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return View(movie);
        }

        public async Task<IActionResult> History()
        {
            var history = await _context.WatchHistories
                .Include(h => h.Movie)
                .Where(h => h.UserId == 1)
                .OrderByDescending(h => h.LastWatched)
                .ToListAsync();

            return View(history);
        }

        public async Task<IActionResult> Genres()
        {
            // Завантажуємо жанри з бази
            var genres = await _context.Genres.ToListAsync();
            return View(genres);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromHistory(int movieId)
        {
            var historyEntry = await _context.WatchHistories 
                .FirstOrDefaultAsync(h => h.UserId == 1 && h.MovieId == movieId);

            if (historyEntry != null)
            {
                _context.WatchHistories.Remove(historyEntry);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(History));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Завантажуємо всі жанри для відображення у формі
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, int[] selectedGenres)
        {
            if (ModelState.IsValid)
            {
                // Якщо користувач вибрав якісь жанри
                if (selectedGenres != null && selectedGenres.Length > 0)
                {
                    // Знаходимо ці жанри в базі і додаємо до фільму
                    // УВАГА: Перевір, щоб тут була твоя правильна назва (Genres або MovieGenres)
                    movie.Genres = await _context.Genres
                        .Where(g => selectedGenres.Contains(g.Id))
                        .ToListAsync();
                }

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View(movie);
        }

    }
}
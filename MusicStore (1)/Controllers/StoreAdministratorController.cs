using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicStore__1_.Models;

namespace MusicStore__1_.Controllers
{
    [Authorize(Roles = "Manager")]
    public class StoreAdministratorController : Controller
    {
        private readonly MusicStoreEntities _context;

        public StoreAdministratorController(MusicStoreEntities context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var albums = _context.Albums.Include(a => a.Genre).Include(a => a.Artist);
            return View(await albums.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "ArtistId", "ArtistId");
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("GenreId,ArtistId,Title,Price,AlbumArtUrl")]Album album) 
        {
            if (ModelState.IsValid)
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState?[key]?.Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Model state error in property '{key}': {error.ErrorMessage}");
                }
            }
            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "ArtistId", "Artist.Id", album.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId", album.GenreId);
            return View(album);
        }

        public async Task<IActionResult> Edit (int? id)
        {
            if(id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "ArtistId", "Name", album.AlbumId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", album.GenreId);

            return View(album);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, [Bind("AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")]Album album)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Albums.Update(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ArtistId"] = new SelectList(_context.Set<Artist>(), "ArtistId", "ArtistId", album.AlbumId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId", album.GenreId);
            return View(album) ;
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound(ModelState);
            }
            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }
    }
}

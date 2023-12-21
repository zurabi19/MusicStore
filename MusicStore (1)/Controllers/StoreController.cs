using Microsoft.AspNetCore.Mvc;
using System;
using System.Web;
using MusicStore__1_.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MusicStore__1_.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        private readonly MusicStoreEntities _context;

        public StoreController(MusicStoreEntities context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres.ToListAsync();
            return View(genres);
        }

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = _context.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            return View(genreModel);
        }

        public IActionResult Details(int id)
        {
            var album = _context.Albums.Find(id);
            return View(album);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicServiceDomain.Model;
using MusicServiceInfrastructure.ViewModel;

namespace MusicServiceInfrastructure.Controllers
{
    public class Songs1Controller : Controller
    {
        private readonly DbsongsContext _context;

        public Songs1Controller(DbsongsContext context)
        {
            _context = context;
        }

        // GET: Songs1
        public async Task<IActionResult> Index(int? id, string? name)
        {
            var model = _context.Songs
                .Where(b => b.ArtistId == id)
                .Select(song => new SongsViewModel
                {
                    Id = song.Id,
                    Title = song.Title,
                    ArtistName = _context.Artists.FirstOrDefault(artist => artist.Id == song.ArtistId).Name,
                    GenreName = _context.Genres.FirstOrDefault(genre => genre.Id == song.GenreId).Name,
                    LyricsText = _context.Lyrics.FirstOrDefault(lyric => lyric.Id == song.LyricsId).Text,
                    Duration = song.Duration
                })
                .ToList();

            if (id == null)
            {
                return RedirectToAction("Index", "Artists");
            }

            ViewBag.ArtistId = id;
            ViewBag.Name = name;

            return View(model);
        }

        // GET: Songs1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Songs1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ArtistId,GenreId,LyricsId,Duration,Id")] Song song)
        {
            if (ModelState.IsValid)
            {
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(song);
        }

        // GET: Songs1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        // POST: Songs1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,ArtistId,GenreId,LyricsId,Duration,Id")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(song);
        }

        // GET: Songs1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicServiceDomain.Model;
using MusicServiceInfrastructure;
using MusicServiceInfrastructure.ViewModel;

namespace MusicServiceInfrastructure.Controllers
{
    public class SongsController : Controller
    {
        private readonly ILogger<SongsController> _logger;
        private readonly DbsongsContext _context;
        private readonly UserManager<User> _userManager;

        public SongsController(ILogger<SongsController> logger, DbsongsContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var model = _context.Songs
                .Select(song => new SongsViewModel
                {
                    Id = song.Id,
                    Title = song.Title,
                    ArtistName = _context.Artists.FirstOrDefault(artist => artist.Id == song.ArtistId).Name,
                    GenreName = _context.Genres.FirstOrDefault(genre => genre.Id == song.GenreId).Name,
                    LyricsText = _context.Lyrics.FirstOrDefault(lyric => lyric.Id == song.LyricsId).Text,
                    Duration = song.Duration,
                    IsLikedByCurrentUser = song.Likes.Any(l => l.UserId == userId)
                })
                .ToList();

            return View(model);
        }

        // GET: Songs/Details/5
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

            var songViewModel = new SongsViewModel
            {
                Id = song.Id,
                Title = song.Title,
                ArtistName = _context.Artists.FirstOrDefault(artist => artist.Id == song.ArtistId).Name,
                GenreName = _context.Genres.FirstOrDefault(genre => genre.Id == song.GenreId).Name,
                LyricsText = _context.Lyrics.FirstOrDefault(lyric => lyric.Id == song.LyricsId)?.Text,
                Duration = song.Duration
            };

            return View(songViewModel);
        }

        // GET: Songs/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            var unusedLyrics = _context.Lyrics.Where(l => !_context.Songs.Any(s => s.LyricsId == l.Id)).ToList();
            ViewData["LyricsId"] = new SelectList(unusedLyrics, "Id", "Text");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
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

        // GET: Songs/Edit/5
        [Authorize(Roles = "admin")]
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
            ViewData["ArtistId"] = new SelectList(_context.Artists, "Id", "Name", song.ArtistId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", song.GenreId); 
            var unusedLyrics = _context.Lyrics.Where(l => !_context.Songs.Any(s => s.LyricsId == l.Id)).ToList();
            ViewData["LyricsId"] = new SelectList(unusedLyrics, "Id", "Text");
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
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

        // GET: Songs/Delete/5
        [Authorize(Roles = "admin")]
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

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            var relatedSongsArtists = await _context.SongsArtists.Where(sa => sa.SongId == id).ToListAsync();
            _context.SongsArtists.RemoveRange(relatedSongsArtists);

            var relatedSongsGenres = await _context.SongsGenres.Where(sg => sg.SongId == id).ToListAsync();
            _context.SongsGenres.RemoveRange(relatedSongsGenres);

            _context.Songs.Remove(song);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int songId)
        {
            var user = await _userManager.GetUserAsync(User);
            var song = _context.Songs.FirstOrDefault(s => s.Id == songId);

            if (user != null && song != null)
            {
                var like = new Like
                {
                    UserId = user.Id,
                    SongId = songId,
                    LikeDateTime = DateOnly.FromDateTime(DateTime.Now)
            };

                _context.Likes.Add(like);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int songId)
        {
            var user = await _userManager.GetUserAsync(User);
            var like = _context.Likes.FirstOrDefault(l => l.UserId == user.Id && l.SongId == songId);

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await fileExcel.CopyToAsync(stream);
                        stream.Position = 0;

                        using (XLWorkbook workBook = new XLWorkbook(stream))
                        {
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        Song song = new Song();
                                        Artist newArtist;
                                        Genre newGenre;
                                        Lyric newLyrics;

                                        string songTitle = row.Cell(1).Value.ToString();
                                        string artistName = row.Cell(2).Value.ToString();
                                        string genreName = row.Cell(3).Value.ToString();
                                        string lyricsText = row.Cell(4).Value.ToString();
                                        song.Duration = (int)row.Cell(5).Value;

                                        var existingTitle = await _context.Songs.FirstOrDefaultAsync(a => a.Title == songTitle);
                                        var existingArtist = await _context.Artists.FirstOrDefaultAsync(a => a.Name == artistName);

                                        if (existingArtist != null && existingTitle != null)
                                        {
                                        }
                                        else
                                        {
                                            song.Title = songTitle;
                                            if (existingArtist == null)
                                            {
                                                newArtist = new Artist { Name = artistName };
                                                _context.Artists.Add(newArtist);
                                                await _context.SaveChangesAsync();
                                                song.ArtistId = newArtist.Id;
                                            }
                                            else
                                            {
                                                song.ArtistId = existingArtist.Id;
                                            }

                                            var existingGenre = await _context.Genres.FirstOrDefaultAsync(a => a.Name == genreName);
                                            if (existingGenre == null)
                                            {
                                                newGenre = new Genre { Name = genreName };
                                                _context.Genres.Add(newGenre);
                                                await _context.SaveChangesAsync();
                                                song.GenreId = newGenre.Id;
                                            }
                                            else
                                            {
                                                song.GenreId = existingGenre.Id;
                                            }

                                            _context.Songs.Add(song);
                                            await _context.SaveChangesAsync();

                                            var existingLyrics = await _context.Lyrics.FirstOrDefaultAsync(a => a.Text == lyricsText);

                                            if (existingLyrics == null)
                                            {
                                                newLyrics = new Lyric { Text = lyricsText, SongId = song.Id };
                                                _context.Lyrics.Add(newLyrics);
                                                await _context.SaveChangesAsync();
                                                song.LyricsId = newLyrics.Id;
                                            }
                                            else
                                            {
                                                if (existingLyrics.SongId == null)
                                                {
                                                    existingLyrics.SongId = song.Id;
                                                    await _context.SaveChangesAsync();
                                                    song.LyricsId = existingLyrics.Id;
                                                }
                                                else
                                                {
                                                    newLyrics = new Lyric { Text = lyricsText, SongId = song.Id };
                                                    _context.Lyrics.Add(newLyrics);
                                                    await _context.SaveChangesAsync();
                                                    song.LyricsId = newLyrics.Id;
                                                }
                                            }

                                            SongsArtist sa = new SongsArtist();
                                            sa.SongId = song.Id;
                                            sa.ArtistId = song.ArtistId;
                                            _context.SongsArtists.Add(sa);

                                            SongsGenre sg = new SongsGenre();
                                            sg.SongId = song.Id;
                                            sg.GenreId = song.GenreId;
                                            _context.SongsGenres.Add(sg);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e, "Помилка при імпорті даних з файлу Excel.");
                                    }
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Export()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var songs = _context.Songs.Include(s => s.SongsArtists).ThenInclude(sa => sa.Artist).ToList();
                var worksheet = workbook.Worksheets.Add("Пісні");

                worksheet.Cell("A1").Value = "Назва";
                worksheet.Cell("B1").Value = "Виконавець";
                worksheet.Cell("C1").Value = "Жанр";
                worksheet.Cell("D1").Value = "Текст";
                worksheet.Cell("E1").Value = "Тривалість";
                worksheet.Row(1).Style.Font.Bold = true;

                for (int i = 0; i < songs.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = songs[i].Title;
                    worksheet.Cell(i + 2, 2).Value = await _context.Artists
                        .Where(a => a.Id == songs[i].ArtistId)
                        .Select(a => a.Name)
                        .FirstOrDefaultAsync();
                    worksheet.Cell(i + 2, 3).Value = await _context.Genres
                        .Where(a => a.Id == songs[i].GenreId)
                        .Select(a => a.Name)
                        .FirstOrDefaultAsync();
                    worksheet.Cell(i + 2, 4).Value = await _context.Lyrics
                        .Where(a => a.Id == songs[i].LyricsId)
                        .Select(a => a.Text)
                        .FirstOrDefaultAsync();
                    worksheet.Cell(i + 2, 5).Value = songs[i].Duration;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"musicService_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

    }
}

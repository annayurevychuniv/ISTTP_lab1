using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Humanizer.Localisation;
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

        public SongsController(ILogger<SongsController> logger, DbsongsContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var model = _context.Songs
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                                        song.Title = row.Cell(1).Value.ToString();

                                        string artistName = row.Cell(2).Value.ToString();
                                        string albumName = row.Cell(3).Value.ToString();

                                        var existingArtist = await _context.Artists.FirstOrDefaultAsync(a => a.Name == artistName);
                                        if (existingArtist == null)
                                        {
                                            Artist newArtist = new Artist { Name = artistName };
                                            _context.Artists.Add(newArtist);
                                            await _context.SaveChangesAsync();
                                            song.ArtistId = newArtist.Id;
                                        }
                                        else
                                        {
                                            song.ArtistId = existingArtist.Id;
                                        }

                                        _context.Songs.Add(song);
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



        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var songs = _context.Songs.Include(s => s.SongsArtists).ThenInclude(sa => sa.Artist).ToList();
                var worksheet = workbook.Worksheets.Add("Пісні");

                worksheet.Cell("A1").Value = "Назва";
                worksheet.Cell("B1").Value = "Виконавець";
                worksheet.Cell("D1").Value = "Жанр";
                worksheet.Cell("E1").Value = "Текст";
                worksheet.Cell("F1").Value = "Тривалість";
                worksheet.Row(1).Style.Font.Bold = true;

                for (int i = 0; i < songs.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = songs[i].Title;
                    worksheet.Cell(i + 2, 2).Value = songs[i].SongsArtists.FirstOrDefault()?.Artist.Name;
                    worksheet.Cell(i + 2, 4).Value = songs[i].GenreId;
                    worksheet.Cell(i + 2, 5).Value = songs[i].LyricsId;
                    worksheet.Cell(i + 2, 6).Value = songs[i].Duration;
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

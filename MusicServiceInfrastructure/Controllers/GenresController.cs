using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicServiceDomain.Model;
using MusicServiceInfrastructure;
using ClosedXML.Excel;

namespace MusicServiceInfrastructure.Controllers
{
    public class GenresController : Controller
    {
        private readonly DbsongsContext _context;

        public GenresController(DbsongsContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Songs", new { id = genre.Id, name = genre.Name });
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Id")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Id")] Genre genre)
        {
            if (id != genre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.Id))
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
            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream))
                        {
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                Genre newgen;
                                var c = (from gen in _context.Genres
                                         where gen.Name.Contains(worksheet.Name)
                                         select gen).ToList();
                                if (c.Count > 0)
                                {
                                    newgen = c[0];
                                }
                                else
                                {
                                    newgen = new Genre();
                                    newgen.Name = worksheet.Name;
                                    newgen.Description = "from EXCEL";
                                    _context.Genres.Add(newgen);
                                }                
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        Song song = new Song();
                                        song.Title = row.Cell(1).Value.ToString();
                                        song.GenreId = newgen.Id;
                                        _context.Songs.Add(song);
                                        for (int i = 2; i <= 5; i++)
                                        {
                                            if (row.Cell(i).Value.ToString().Length > 0)
                                            {
                                                Artist artist;

                                                var a = (from art in _context.Artists
                                                         where art.Name.Contains(row.Cell(i).Value.ToString())
                                                         select art).ToList();
                                                if (a.Count > 0)
                                                {
                                                    artist = a[0];
                                                }
                                                else
                                                {
                                                    artist = new Artist();
                                                    artist.Name = row.Cell(i).Value.ToString();
                                                    _context.Add(artist);
                                                }
                                                SongsArtist sa = new SongsArtist();
                                                sa.Song = song;
                                                sa.Artist = artist;
                                                _context.SongsArtists.Add(sa);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        //logging самостійно :)

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
                var genres = _context.Genres.Include("SongsGenres").ToList();
                foreach (var g in genres)
                {
                    var worksheet = workbook.Worksheets.Add(g.Name);

                    worksheet.Cell("A1").Value = "Назва";
                    worksheet.Cell("B1").Value = "Автор 1";
                    worksheet.Cell("C1").Value = "Автор 2";
                    worksheet.Cell("D1").Value = "Автор 3";
                    worksheet.Cell("E1").Value = "Автор 4";
                    worksheet.Cell("F1").Value = "Категорія";
                    worksheet.Cell("G1").Value = "Інформація";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var songs = g.SongsGenres.ToList();

                    for (int i = 0; i < songs.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = songs[i].SongId;
                        worksheet.Cell(i + 2, 7).Value = songs[i].GenreId;

                        var sa = _context.SongsArtists.Where(a => a.SongId == songs[i].Id).Include("Artist").ToList();


                        int j = 0;
                        foreach (var a in sa)
                        {
                            if (j < 5)
                            {
                                worksheet.Cell(i + 2, j + 2).Value = a.Artist.Name;
                                j++;
                            }
                        }

                    }
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

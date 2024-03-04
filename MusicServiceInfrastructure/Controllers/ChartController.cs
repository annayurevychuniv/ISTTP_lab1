using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicServiceDomain.Model;

namespace MusicServiceInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DbsongsContext _context;
        public ChartController(DbsongsContext context)
        {
            _context = context;
        }

        [HttpGet("Genres")]
        public JsonResult Genres()
        {
            var genres = _context.Genres.ToList();
            List<object> genSong = new List<object>();
            genSong.Add(new[] { "Жанр", "Кількість пісень" });

            foreach (var gen in genres)
            {
                var songCount = _context.SongsGenres.Count(sg => sg.GenreId == gen.Id);
                genSong.Add(new object[] { gen.Name, songCount });
            }

            return new JsonResult(genSong);
        }

        [HttpGet("Artists")]
        public JsonResult Artists()
        {
            var artists = _context.Artists.ToList();
            List<object> artSong = new List<object>();
            artSong.Add(new[] { "Виконавець", "Кількість пісень" });

            foreach (var art in artists)
            {
                var songCount = _context.SongsArtists.Count(sa => sa.ArtistId == art.Id);
                artSong.Add(new object[] { art.Name, songCount });
            }

            return new JsonResult(artSong);
        }

    }
}

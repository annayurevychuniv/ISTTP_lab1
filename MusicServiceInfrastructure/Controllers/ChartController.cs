using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var genres = _context.Genres.ToList();
            List<object> catSong = new List<object>();
            catSong.Add(new[] { "Жанр", "Кількість пісень" });
            foreach (var c in genres)
            {
                catSong.Add(new object[] { c.Name, c.SongsGenres.Count() });
            }
            return new JsonResult(catSong);
        }
    }
}

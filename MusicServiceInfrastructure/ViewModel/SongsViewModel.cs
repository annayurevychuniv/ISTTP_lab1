using MusicServiceDomain.Model;
using System.ComponentModel.DataAnnotations;

namespace MusicServiceInfrastructure.ViewModel
{
    public class SongsViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Назва")]
        public string Title { get; set; }

        [Display(Name = "Виконавець")]
        public string ArtistName { get; set; }

        [Display(Name = "Жанр")]
        public string GenreName { get; set; }

        [Display(Name = "Текст")]
        public string LyricsText { get; set; }

        [Display(Name = "Тривалість")]
        public int Duration { get; set; }

        public bool? IsLikedByCurrentUser { get; set; }
    }
}

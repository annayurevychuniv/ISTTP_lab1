using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicServiceDomain.Model;

public partial class Song : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Виконавець")]
    public int ArtistId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Альбом")]
    public int AlbumId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Жанр")]
    public int GenreId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Текст")]
    public int LyricsId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Тривалість")]
    public int Duration { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual Lyric? Lyric { get; set; }

    public virtual ICollection<SongsArtist> SongsArtists { get; set; } = new List<SongsArtist>();

    public virtual ICollection<SongsGenre> SongsGenres { get; set; } = new List<SongsGenre>();
}

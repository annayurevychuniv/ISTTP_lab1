﻿using System;
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
    [Display(Name = "Жанр")]
    public int GenreId { get; set; }

    [Display(Name = "Текст")]
    public int LyricsId { get; set; }

    [Range(1, 6000, ErrorMessage = "Тривалість повинна бути більше 0 і менше 6000 с")]
    [Display(Name = "Тривалість (в секундах)")]
    public int Duration { get; set; }

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual Lyric? Lyric { get; set; }

    public virtual ICollection<SongsArtist> SongsArtists { get; set; } = new List<SongsArtist>();

    public virtual ICollection<SongsGenre> SongsGenres { get; set; } = new List<SongsGenre>();
}
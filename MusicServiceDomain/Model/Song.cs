using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class Song : Entity
{

    public string Title { get; set; } = null!;

    public int ArtistId { get; set; }

    public int AlbumId { get; set; }

    public int GenreId { get; set; }

    public int? LyricsId { get; set; }

    public int Duration { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual Lyric? Lyric { get; set; }

    public virtual ICollection<SongsArtist> SongsArtists { get; set; } = new List<SongsArtist>();

    public virtual ICollection<SongsGenre> SongsGenres { get; set; } = new List<SongsGenre>();
}

using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class SongsGenre : Entity
{
    public int GenreId { get; set; }

    public int SongId { get; set; }

    public virtual Genre Genre { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;
}
using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class SongsArtist : Entity
{
    public int ArtistId { get; set; }

    public int SongId { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;
}
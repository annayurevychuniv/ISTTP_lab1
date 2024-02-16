using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class Lyric : Entity
{

    public string Text { get; set; } = null!;

    public int SongId { get; set; }

    public virtual Song Song { get; set; } = null!;
}

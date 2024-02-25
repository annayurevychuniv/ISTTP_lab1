using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class Like : Entity
{
    public int SongId { get; set; }

    public string UserId { get; set; } = null!;

    public DateOnly LikeDateTime { get; set; }

    public virtual Song Song { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
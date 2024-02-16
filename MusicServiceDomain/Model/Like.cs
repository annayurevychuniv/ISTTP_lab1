using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class Like : Entity
{

    public int SongId { get; set; }

    public int UserId { get; set; }

    public DateOnly LikeDateTime { get; set; }

    public virtual Song Song { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

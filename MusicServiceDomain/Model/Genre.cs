using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class Genre : Entity
{

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<SongsGenre> SongsGenres { get; set; } = new List<SongsGenre>();
}

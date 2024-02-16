using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class Artist : Entity
{

    public string Name { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<SongsArtist> SongsArtists { get; set; } = new List<SongsArtist>();
}

using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class Album : Entity
{

    public string Title { get; set; } = null!;

    public DateOnly? ReleaseYear { get; set; }

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}

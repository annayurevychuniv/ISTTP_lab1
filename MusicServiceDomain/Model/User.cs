using System;
using System.Collections.Generic;

namespace MusicServiceDomain.Model;

public partial class User : Entity
{

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}

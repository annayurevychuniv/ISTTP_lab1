using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MusicServiceDomain.Model;

public partial class User : IdentityUser
{
    public int Year { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}

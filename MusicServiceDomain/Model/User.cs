using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MusicServiceDomain.Model;

public partial class User : IdentityUser
{
    public string? Name { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
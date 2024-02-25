using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicServiceDomain.Model;

public partial class Genre : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Жанр")]
    public string Name { get; set; } = null!;

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    public virtual ICollection<SongsGenre> SongsGenres { get; set; } = new List<SongsGenre>();
}
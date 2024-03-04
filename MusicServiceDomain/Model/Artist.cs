using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicServiceDomain.Model;

public partial class Artist : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;

    [Display(Name = "Дата народження")]
    public DateOnly? BirthDate { get; set; }

    [Display(Name = "Країна")]
    public string? Country { get; set; }

    [Display(Name = "Зображення")]
    public byte[]? Image { get; set; }

    public virtual ICollection<SongsArtist> SongsArtists { get; set; } = new List<SongsArtist>();
}

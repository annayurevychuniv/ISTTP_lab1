using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicServiceDomain.Model;

public partial class Album : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Альбом")]
    public string Title { get; set; } = null!;

    [Display(Name = "Рік випуску")]
    public DateOnly? ReleaseYear { get; set; }

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}

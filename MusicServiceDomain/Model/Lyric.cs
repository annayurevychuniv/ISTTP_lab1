using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicServiceDomain.Model;

public partial class Lyric : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Текст")]
    public string Text { get; set; } = null!;

    public int? SongId { get; set; }

    public virtual Song? Song { get; set; }
}
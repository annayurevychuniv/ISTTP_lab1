using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicServiceDomain.Model;

public partial class User : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Пошта")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}

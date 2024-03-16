using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MusicServiceDomain.Model;

public class MaxBirthDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var maxDate = DateOnly.FromDateTime(DateTime.Today);
        var minDate = DateOnly.Parse("1900-01-01");

        if (value is DateOnly dateValue)
        {
            if (dateValue.CompareTo(minDate) < 0 || dateValue.CompareTo(maxDate) > 0)
            {
                return new ValidationResult($"Дата народження може бути від {minDate.ToString("yyyy-MM-dd")} до {maxDate.ToString("yyyy-MM-dd")}");
            }
        }

        return ValidationResult.Success;
    }
}

public partial class Artist : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;

    [Display(Name = "Дата народження (рррр-мм-дд)")]
    [MaxBirthDate(ErrorMessage = "Дата народження може бути від 1900 року до сьогоднішнього дня")]
    public DateOnly? BirthDate { get; set; }

    [Display(Name = "Країна")]
    public string? Country { get; set; }

    [Display(Name = "Зображення")]
    public byte[]? Image { get; set; }

    public virtual ICollection<SongsArtist> SongsArtists { get; set; } = new List<SongsArtist>();
}

using System.ComponentModel.DataAnnotations;

namespace MusicServiceInfrastructure.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Пошта")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запам'ятати?")]
        public bool RememberMe { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace WebUI.Areas.Public.Models
{
    public class ContactVm
    {
        [Required]
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
    }
}
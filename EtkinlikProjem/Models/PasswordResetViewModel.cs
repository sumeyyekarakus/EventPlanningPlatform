using System.ComponentModel.DataAnnotations;

namespace EtkinlikProjem.Models
{
    public class PasswordResetViewModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string YeniSifre { get; set; }

        [Compare("YeniSifre", ErrorMessage = "Şifreler eşleşmiyor")]
        public string SifreOnay { get; set; }
    }
}

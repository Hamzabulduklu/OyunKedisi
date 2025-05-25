using System.ComponentModel.DataAnnotations;

namespace OyunKedisi.Models
{
    public class LoginViewModel
    {
        [Required, Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Şifre")]
        public string SifreHash { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace OyunKedisi.Models
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; }

        [Required, EmailAddress, Display(Name = "E-Posta")]
        public string Mail { get; set; }

        [Display(Name = "Yas")]
        public int? Yas { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Şifre")]
        public string SifreHash { get; set; }

        [Required, DataType(DataType.Password), Compare("SifreHash"), Display(Name = "Şifre (Tekrar)")]
        public string ConfirmSifreHash { get; set; }
    }
}

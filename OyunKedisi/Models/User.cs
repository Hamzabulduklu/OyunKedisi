using System;
using System.Collections.Generic;

namespace OyunKedisi.Models;

public partial class User
{
    public int Id { get; set; }

    public string KullaniciAdi { get; set; } = null!;

    public string SifreHash { get; set; } = null!;

    public string? Mail { get; set; }

    public int? Yas { get; set; }

    public int? YetkiId { get; set; }

    public DateTime KayitTarihi { get; set; }

    public virtual ICollection<Favori> Favoris { get; set; } = new List<Favori>();

    public virtual ICollection<Oyunlar> Oyunlars { get; set; } = new List<Oyunlar>();

    public virtual Yetki? Yetki { get; set; }

    public virtual ICollection<Yorumlar> Yorumlars { get; set; } = new List<Yorumlar>();
}

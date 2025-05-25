using System;
using System.Collections.Generic;

namespace OyunKedisi.Models;

public partial class Oyunlar
{
    public int Id { get; set; }

    public string OyunAdi { get; set; } = null!;

    public int? UserId { get; set; }

    public string? OyunFotograflari { get; set; }

    public string? OyunLinki { get; set; }

    public DateTime EklemeTarihi { get; set; }

    public virtual ICollection<Favori> Favoris { get; set; } = new List<Favori>();

    public virtual User? User { get; set; }

    public virtual ICollection<Yorumlar> Yorumlars { get; set; } = new List<Yorumlar>();
}

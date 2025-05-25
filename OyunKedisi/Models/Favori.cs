using System;
using System.Collections.Generic;

namespace OyunKedisi.Models;

public partial class Favori
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? OyunId { get; set; }

    public virtual Oyunlar? Oyun { get; set; }

    public virtual User? User { get; set; }
}

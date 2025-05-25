using System;
using System.Collections.Generic;

namespace OyunKedisi.Models;

public partial class Yorumlar
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? OyunId { get; set; }

    public string? Yorumlar1 { get; set; }

    public virtual Oyunlar? Oyun { get; set; }

    public virtual User? User { get; set; }
}

using System;
using System.Collections.Generic;

namespace OyunKedisi.Models;

public partial class Yetki
{
    public int Id { get; set; }

    public string YetkiAdi { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

using System;
using System.Collections.Generic;

namespace OyunKedisi.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<Oyunlar> Games { get; set; }
        public List<Favori> Favorites { get; set; }
    }
} 
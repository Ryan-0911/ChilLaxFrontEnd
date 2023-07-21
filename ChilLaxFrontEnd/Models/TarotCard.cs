using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class TarotCard
    {
        public int TarotCardId { get; set; }
        public string TarotCardName { get; set; } = null!;
        public string TarotCardType { get; set; } = null!;
    }
}

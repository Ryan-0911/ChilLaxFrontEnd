using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class TarotOrder
{
    public int TarotOrderId { get; set; }

    public int MemberId { get; set; }

    public string CardResult { get; set; } = null!;

    public string DivinationChat { get; set; } = null!;

    public DateTime TarotTime { get; set; }

    public virtual Member Member { get; set; } = null!;
}

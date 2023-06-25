using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class PointHistory
{
    public int PointDetailId { get; set; }

    public int MemberId { get; set; }

    public string ModifiedSource { get; set; } = null!;

    public int ModifiedAmount { get; set; }

    public virtual Member Member { get; set; } = null!;
}

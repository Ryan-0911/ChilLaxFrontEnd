using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class MemberCredential
{
    public int MemberId { get; set; }

    public string MemberAccount { get; set; } = null!;

    public string MemberPassword { get; set; } = null!;

    public bool Available { get; set; }

    public virtual Member Member { get; set; } = null!;
}

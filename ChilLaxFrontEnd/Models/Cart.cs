using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class Cart
{
    public int MemberId { get; set; }

    public int ProductId { get; set; }

    public int CartProductQuantity { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

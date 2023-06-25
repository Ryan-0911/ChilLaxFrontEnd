﻿using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class OrderDetail
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int CartProductQuantity { get; set; }

    public virtual ProductOrder Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

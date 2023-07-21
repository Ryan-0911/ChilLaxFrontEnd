using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string SupplierTel { get; set; } = null!;

    public string SupplierAddress { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}

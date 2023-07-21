using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public int SupplierId { get; set; }

    public string? PurchaseNote { get; set; }

    public DateTime PurchaseDate { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}

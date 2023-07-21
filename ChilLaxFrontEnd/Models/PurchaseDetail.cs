using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class PurchaseDetail
    {
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int PurchaseQuantity { get; set; }
        public int PurchasePrice { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Purchase Purchase { get; set; } = null!;
    }
}

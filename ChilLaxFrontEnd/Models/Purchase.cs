using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class Purchase
    {
        public Purchase()
        {
            PurchaseDetails = new HashSet<PurchaseDetail>();
        }

        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public string? PurchaseNote { get; set; }
        public DateTime PurchaseDate { get; set; }

        public virtual Supplier Supplier { get; set; } = null!;
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}

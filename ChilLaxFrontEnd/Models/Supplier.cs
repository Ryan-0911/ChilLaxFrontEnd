using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Products = new HashSet<Product>();
            Purchases = new HashSet<Purchase>();
        }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public string SupplierTel { get; set; } = null!;
        public string SupplierAddress { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}

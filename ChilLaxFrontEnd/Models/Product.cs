using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int SupplierId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductDesc { get; set; } = null!;

    public int ProductPrice { get; set; }

    public string ProductImg { get; set; } = null!;

    public int ProductQuantity { get; set; }

    public string ProductCategory { get; set; } = null!;

    public bool ProductState { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}

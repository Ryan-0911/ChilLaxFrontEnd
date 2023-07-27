﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class Product
    {
        public Product()
        {
            Cart = new HashSet<Cart>();
            OrderDetail = new HashSet<OrderDetail>();
            PurchaseDetail = new HashSet<PurchaseDetail>();
        }

        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public int ProductPrice { get; set; }
        public string ProductImg { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductCategory { get; set; }
        public bool ProductState { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<PurchaseDetail> PurchaseDetail { get; set; }
    }
}
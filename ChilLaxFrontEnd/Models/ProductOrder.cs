using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class ProductOrder
    {
        public ProductOrder()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string OrderId { get; set; } = null!;
        public int MemberId { get; set; }
        public bool OrderPayment { get; set; }
        public int OrderTotalPrice { get; set; }
        public bool OrderDelivery { get; set; }
        public string OrderAddress { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string? OrderNote { get; set; }
        public string OrderState { get; set; } = null!;

        public virtual Member Member { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

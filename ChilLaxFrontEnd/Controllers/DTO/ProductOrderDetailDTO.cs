using ChilLaxFrontEnd.Models;
using System.ComponentModel;

namespace ChilLaxFrontEnd.Controllers.DTO
{
    public class ProductOrderDetailDTO
    {
        ChilLaxContext db = new ChilLaxContext();

        public ProductOrder? ProductOrder { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        public Product? Product { get; set; }

    
    }
}
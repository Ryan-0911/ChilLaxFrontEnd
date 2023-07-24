using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChilLaxFrontEnd.Models.DTO
{
    public class ProductsPagingDTO
    {
        public int? pageCount { get; set; }
        public int? nowpage { get; set; }
        public IEnumerable<SelectListItem> ProductSelectedList { get; set; }


        public int TotalPages { get; set; }
        public List<Product> ProductsResult { get; set; }
    }
}

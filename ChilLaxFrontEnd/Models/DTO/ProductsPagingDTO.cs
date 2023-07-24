namespace ChilLaxFrontEnd.Models.DTO
{
    public class ProductsPagingDTO
    {
        public int TotalPages { get; set; }
        public List<Product> ProductsResult { get; set; }
    }
}

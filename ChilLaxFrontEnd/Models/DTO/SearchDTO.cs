namespace ChilLaxFrontEnd.Models.DTO
{
    public class SearchDTO
    {
        public string? keyword { get; set; }
        public string? sortBy { get; set; }
        public string? sortType { get; set; }
        public int? page { get; set; }
    }
}

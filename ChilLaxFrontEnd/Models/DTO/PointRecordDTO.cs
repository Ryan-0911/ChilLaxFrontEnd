namespace ChilLaxFrontEnd.Models.DTO
{
    public class PointRecordDTO
    {
        public string ModifiedSource { get; set; } = null!;
        public string ModifiedContent { get; set; } = null!;
        public int ModifiedAmount { get; set; }
        public string ModifiedTime { get; set; } = null!;
    }
}

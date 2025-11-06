namespace AppForSEII2526.API.DTOs.PlanDTOs
{
    public class PlanItemForDetailDTO
    {
        public int ClassId { get; set; }
        public string Name { get; set; }
        public IList<string> Type { get; set; } // ["Yoga", "Spinning"]

        [Precision(9, 2)]
        public decimal Price { get; set; } // price paid
        public DateTime Date { get; set; }
        public string? Goal { get; set; }
    }
}

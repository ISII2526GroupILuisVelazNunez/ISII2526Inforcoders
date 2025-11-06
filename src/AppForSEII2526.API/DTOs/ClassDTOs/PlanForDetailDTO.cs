namespace AppForSEII2526.API.DTOs.PlanDTOs
{
    public class PlanForDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Weeks { get; set; }
        public string? HealthIssues { get; set; }

        [Precision(9, 2)]
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserFullName { get; set; } // "Nombre Apellido"

        public IList<PlanItemForDetailDTO> PlanItems { get; set; }
    }
}
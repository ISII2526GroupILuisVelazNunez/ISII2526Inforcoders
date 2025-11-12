namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PaymentMethodForPurchaseDetailDTO
    {
        public PaymentMethodForPurchaseDetailDTO(int id, string type)
        {
            Id = id;
            Type = type;
        }
        public int Id { get; set; }
        public string Type { get; set; }
    }
}

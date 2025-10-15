namespace AppForSEII2526.API.DTOs.ItemDTOs
{
    public class ItemForPurchaseDTO
    {
        public ItemForPurchaseDTO(int id, string name, string brandName)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        [StringLength(30, ErrorMessage = "The name of the Item can be neither longer than 30 characters nor shorter than 1",
       MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
        public string Brand { get; set; }
    }
}

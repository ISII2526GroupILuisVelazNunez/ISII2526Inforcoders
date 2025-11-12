namespace AppForSEII2526.API.DTOs.ClassDTOs
{
    public class ClassForPlanDTO : IEquatable<ClassForPlanDTO> // IEquatable implementation to use equals
    {
        public ClassForPlanDTO(int id, string name, decimal price, IList<string> typeItems, DateTime date)
        {
            Id = id;
            Name = name;
            Price = price;
            TypeItems = typeItems;
            Date = date;
        }

        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "The name of the Plan can be neither longer than 20 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Name { get; set; }

        [Precision(9, 2)]
        public decimal Price { get; set; }
        public IList<string> TypeItems { get; set; }
        public DateTime Date { get; set; }


        public bool Equals(ClassForPlanDTO? other)
        {
            if (other is null)
                return false;

            return Id == other.Id &&
                   Name == other.Name &&
                   Price == other.Price &&
                   Date == other.Date &&
                   TypeItems.SequenceEqual(other.TypeItems); // comparing lists
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ClassForPlanDTO);
        }

        public override int GetHashCode()
        {
            // basic so it works
            return HashCode.Combine(Id, Name, Price, Date);
        }
    }
}

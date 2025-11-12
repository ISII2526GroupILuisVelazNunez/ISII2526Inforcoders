namespace AppForSEII2526.API.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Class
    {
        public Class()
        {
            // necessary, ignore warnings
        }

        
        public Class(string name, DateTime date, int capacity, decimal price)
        {
            Name = name;
            Date = date;
            Capacity = capacity;
            Price = price;
        }

        // custom constructor with typeItems for tests, calls previous constructor
        public Class(string name, DateTime date, int capacity, decimal price, IList<TypeItem> typeItems)
            : this(name, date, capacity, price) 
        {
            TypeItems = typeItems;
        }


        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "The name of the Plan can be neither longer than 20 characters nor shorter than 1",
        MinimumLength = 1)]
        [Required]
        public string Name { get; set; }

        [Precision(9, 2)]
        public decimal Price { get; set; }
        public IList<PlanItem> PlanItems { get; set; } // Class relationship with PlanItem
        public IList<TypeItem> TypeItems { get; set; } // Class relationship with TypeItem
    }
}

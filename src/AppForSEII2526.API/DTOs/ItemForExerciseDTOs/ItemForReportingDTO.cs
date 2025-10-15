namespace AppForSEII2526.API.DTOs.ItemForExerciseDTOs
{
    public class ItemForReportingDTO
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }

        /*
        public string Name { get; set; }
        public string Description { get; set; }

        public ItemForReportingDTO(int id, string name, string location, string description, string type)
        {
            Id = id;
            Name = name;
            Location = location;
            Description = description;
            Type = type;
        }
        */

        public ItemForReportingDTO(int id, string location, string typeName)
        {
            Id = id;
            Location = location;
            Type = typeName;
        }
    }
}

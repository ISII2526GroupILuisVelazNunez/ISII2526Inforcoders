namespace AppForSEII2526.API.DTOs.ItemForExerciseDTOs
{
    public class ItemForReportingDTO
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ItemForReportingDTO(int id, string name, string location, string description, string typeName)
        {
            Id = id;
            Name = name;
            Location = location;
            Description = description;
            Type = typeName;
        }

        public ItemForReportingDTO(string name, string location, string description, string typeName)
        {
            Name = name;
            Location = location;
            Description = description;
            Type = typeName;
        }

        public override bool Equals(object? obj)
        {
            return obj is ItemForReportingDTO dTO &&
                   Id == dTO.Id &&
                   Location == dTO.Location &&
                   Type == dTO.Type &&
                   Name == dTO.Name &&
                   Description == dTO.Description;
        }
    }
}

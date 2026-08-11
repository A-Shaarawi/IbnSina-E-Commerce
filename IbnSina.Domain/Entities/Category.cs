namespace IbnSina.Domain.Entities
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        
        public DateTime? CreatedAt { get; private set; }
        private Category() { }
        public Category( string name, string? description = null) {
            SetName(name);
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.");
            Name = name;
        }
        public void UpdateDetails(string name, string? description)
        {
            SetName(name);
            Description = description;
        }
    }   
}

#pragma warning disable CS8618
#pragma warning disable S101

namespace DTO
{
    public class Category
    {
        // Primary Key
        public int ID { get; set; }

        // Properties
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string? Description { get; set; }

        // External Properties
        public ICollection<CategoryItem> CategoryItems { get; set; }

        // Foreign Keys
        public int MenuID { get; set; }

        // Navigational Properties
        public virtual Menu Menu { get; set; }
    }
}

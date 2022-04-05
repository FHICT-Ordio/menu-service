#pragma warning disable CS8618
#pragma warning disable S101

using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class Menu
    {
        // Constructor
        public Menu()
        {
            Items = new List<Item>();
            Categories = new List<Category>();
        }

        // Primary Key
        public int ID { get; set; }

        // Properties
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string RestaurantName { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastEdited { get; set; }

        // External Properties
        public ICollection<Item> Items { get; set; }
        public ICollection<Category> Categories { get; set; }

        // Foreign Keys

        // Navigational Properties
    }
}

#pragma warning disable CS8618
#pragma warning disable S101

using System.ComponentModel.DataAnnotations;

using DTO;

namespace DAL.Model
{
    public class Menu
    {
        // Constructor
        public Menu()
        {
            Items = new List<Item>();
            Categories = new List<Category>();
        }

        public Menu(MenuDTO dto)
        {
            

            ID = dto.ID;
            Owner = dto.Owner;
            Title = dto.Title;
            RestaurantName = dto.RestaurantName;
            Description = dto.Description;
            LastEdited = dto.LastEdited;
            Archived = dto.Archived;

            Items = new List<Item>();
            foreach (ItemDTO item in dto.Items)
            {
                Items.Add(new Item(item));
            }

            Categories = new List<Category>();
            foreach (CategoryDTO category in dto.Categories)
            {
                Categories.Add(new Category(category));
            }
        }

        // Primary Key
        public int ID { get; set; }

        // Properties
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Owner { get; set; }

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
        public bool Archived { get; set; }

        // External Properties
        public ICollection<Item> Items { get; set; }
        public ICollection<Category> Categories { get; set; }

        // Foreign Keys

        // Navigational Properties

        // Methods
        public MenuDTO ToDTO(bool keepReferences = false, bool keepItemReferences = false, bool keepCategoryReferences = false)
        {
            List<ItemDTO> _items = new();            
            List<CategoryDTO> _categories = new();            

            if (keepReferences)
            {
                foreach (Item item in Items)
                {
                    _items.Add(item.ToDTO(keepItemReferences));
                }

                foreach (Category category in Categories)
                {
                    _categories.Add(category.ToDTO(keepCategoryReferences));
                }
            }

            return new MenuDTO
            {
                ID = ID,
                Owner = Owner,
                Title = Title,
                RestaurantName = RestaurantName,
                Description = Description,
                LastEdited = LastEdited,
                Items = _items,
                Categories = _categories
            };
        }
    }
}

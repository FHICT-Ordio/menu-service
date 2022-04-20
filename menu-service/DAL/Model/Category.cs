#pragma warning disable CS8618
#pragma warning disable S101

using DTO;

namespace DAL.Model
{
    public class Category
    {
        public Category()
        {
            Items ??= new List<Item>();
        }

        public Category(CategoryDTO dto)
        {
            ID = dto.ID;
            Name = dto.Name;
            Description = dto.Description;

            Items = new List<Item>();
            foreach(ItemDTO item in dto.Items)
            {
                Items.Add(new Item(item));
            }
        }

        // Primary Key
        public int ID { get; set; }

        // Properties
        public string Name { get; set; }
        public string? Description { get; set; }

        // External Properties
        public ICollection<Item> Items { get; set; }

        // Foreign Keys
        public int MenuID { get; set; }

        // Navigational Properties
        public virtual Menu Menu { get; set; }

        // Methods
        public CategoryDTO ToDTO(bool keepReferences = false)
        {
            List<ItemDTO> _items = new();

            if (keepReferences)
            {
                foreach (Item item in Items)
                {
                    _items.Add(item.ToDTO());
                }
            }

            return new CategoryDTO
            {
                ID = ID,
                Name = Name,
                Description = Description,
                Items = _items
            };
        }
    }
}

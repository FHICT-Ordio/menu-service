#pragma warning disable CS8618
#pragma warning disable S101

using DTO;

namespace DAL.Model
{
    public class Item
    {
        // Constructors
        public Item()
        {
            Tags ??= "";
        }

        public Item(ItemDTO dto)
        {           
            ID = dto.ID;
            Name = dto.Name;
            Description = dto.Description;
            Price = dto.Price;
            Archived = dto.Archived;

            Tags = "";
            foreach(string tag in dto.Tags)
            {                
                Tags += tag + " ";
            }
            Tags = Tags.Trim();

            Categories = new List<Category>();
            foreach (CategoryDTO category in dto.Categories)
            {
                Categories.Add(new Category(category));
            }
        }

        // Primary Key
        public int ID { get; set; }

        // Properties
        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public string? Tags { get; set; }
        public bool Archived { get; set; }


        // External Properties
        public ICollection<Category> Categories { get; set; }

        // Foreign Keys
        public int MenuID { get; set; }

        // Navigational Properties
        public Menu Menu { get; set; }

        // Methods
        public ItemDTO ToDTO(bool keepReferences = false)
        {
            List<CategoryDTO> _categories = new();

            if (keepReferences)
            {
                foreach (Category category in Categories)
                {
                    _categories.Add(category.ToDTO());
                }
            }

            return new ItemDTO
            {
                ID = ID,
                Name = Name,
                Description = Description,
                Price = Price,
                Tags = new List<string>((Tags ?? "").Split(' ')),
                Categories = _categories,
                Archived = Archived
            };
        }
    }
}

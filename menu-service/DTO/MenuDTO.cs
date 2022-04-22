#pragma warning disable CS8618
#pragma warning disable S101

using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class MenuDTO
    {
        // Constructor
        public MenuDTO()
        {
            Items ??= new();
            Categories ??= new();
        }
        public int ID { get; set; }

        public string Owner { get; set; }

        public string Title { get; set; }
        public string RestaurantName { get; set; }
        public string? Description { get; set; }
        public DateTime LastEdited { get; set; }


        public List<ItemDTO> Items { get; set; }
        public List<CategoryDTO> Categories { get; set; }

    }
}

#pragma warning disable CS8618
#pragma warning disable S101

namespace DTO
{
    public class CategoryDTO
    {
        public CategoryDTO()
        {
            Items ??= new();
        }

        public int ID { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Archived { get; set; }

        public List<ItemDTO> Items { get; set; }
    }
}

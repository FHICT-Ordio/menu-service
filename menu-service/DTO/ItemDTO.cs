#pragma warning disable CS8618
#pragma warning disable S101

namespace DTO
{
    public class ItemDTO
    {
        public ItemDTO()
        {
            Categories ??= new();
            Tags ??= new();
        }

        public int ID { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }        
        public List<string> Tags { get; set; }

        public List<CategoryDTO> Categories { get; set; }
    }
}

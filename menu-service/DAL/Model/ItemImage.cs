#pragma warning disable CS8618
#pragma warning disable S101

namespace DAL.Model
{
    public class ItemImage
    {
        public int ProductID { get; set; }
        public byte[] Image { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;

using AL;
using DTO;


namespace DAL
{
    public class CategoryDAL : ICategoryCollection
    {
        private readonly MenuContext _context;

        public CategoryDAL(DbContext context)
        {
            _context = context as MenuContext ?? throw new ArgumentNullException(nameof(context));
        }

        public int Add(DTO.Category item, int menuId)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Menus.FirstOrDefault(x => x.ID == menuId).Categories.Add(item);
            _context.SaveChanges();
            return item.ID;
        }
        public bool Delete(int id) => throw new NotImplementedException();
        public DTO.Category? Get(int id) => throw new NotImplementedException();
    }
}

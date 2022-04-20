using Microsoft.EntityFrameworkCore;

using AL;
using DTO;
using DAL.Model;


namespace DAL
{
    public class CategoryDAL : ICategoryCollection
    {
        private readonly MenuContext _context;

        public CategoryDAL(DbContext context)
        {
            _context = context as MenuContext ?? throw new ArgumentNullException(nameof(context));
        }

        public int Add(int menuID, CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
                return 0;

            Menu? menu = _context.Menus.FirstOrDefault(x => x.ID == menuID);

            if (menu == null)
                return 0;
                
            menu.Categories.Add(new Category(categoryDTO));
            _context.SaveChanges();

            return categoryDTO.ID;
        }
        public CategoryDTO? Get(int menuID, int categoryID)
        {
            Menu? menu = _context.Menus
                .Include(x => x.Categories)
                .ThenInclude(x => x.Items)
                .ThenInclude(x => x.Menu)
                .FirstOrDefault(x => x.ID == menuID);

            if (menu == null)
                return null;

            Category? category = menu.Categories.FirstOrDefault(x => x.ID == categoryID);

            if (category == null)
                return null;

            return category.ToDTO(true);
        }

        public bool Update(int menuID, CategoryDTO categoryDTO)
        {
            Menu? menu = _context.Menus
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.ID == menuID);

            if (menu == null)
                return false;

            Category? category = menu.Categories.FirstOrDefault(x => x.ID == categoryDTO.ID);

            if (category == null)
                return false;

            category.Name = categoryDTO.Name;
            category.Description = categoryDTO.Description;

            _context.Categories.Update(category);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int menuID, int categoryID)
        {
            Menu? menu = _context.Menus
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.ID == menuID);

            if (menu == null)
                return false;

            Category? category = menu.Categories.FirstOrDefault(x => x.ID == categoryID);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            return _context.SaveChanges() > 0;
        }
    }
}

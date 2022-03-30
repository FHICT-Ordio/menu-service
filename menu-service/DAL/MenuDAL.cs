using Microsoft.EntityFrameworkCore;

using AL;
using DTO;


namespace DAL
{
    public class MenuDAL : IMenuCollection
    {
        private readonly MenuContext _context;

        public MenuDAL(DbContext context)
        {
            _context = context as MenuContext ?? throw new ArgumentNullException(nameof(context));
        }

        public Menu? Get(int id)
        {
            DTO.Menu? menu = _context.Menus.FirstOrDefault(x => x.ID == id);

            if (menu == null)
            {
                return null;
            }

            _context.Entry(menu).Collection(x => x.Categories).Load();
            _context.Entry(menu).Collection(x => x.Items).Load();
            return menu;
        }

        public int Add(Menu item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Menus.Add(item);
            _context.SaveChanges();

            return item.ID;
        }

        public bool Delete(int id)
        {
            Menu? menuDTO = Get(id);
            if (menuDTO == null)
            {
                return false;
            }

            _context.Menus.Remove(menuDTO);
            _context.SaveChanges();
            
            return true;
        }
    }
}

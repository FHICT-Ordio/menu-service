using Microsoft.EntityFrameworkCore;

using AL;
using DTO;
using DAL.Model;


namespace DAL
{
    public class ItemDAL : IItemCollection
    {
        private readonly MenuContext _context;

        public ItemDAL(DbContext context)
        {
            _context = context as MenuContext ?? throw new ArgumentNullException(nameof(context));
        }

        public int Add(int menuID, ItemDTO itemDTO)
        {
            if (itemDTO == null)
                throw new ArgumentNullException(nameof(itemDTO));

            Menu? menu = _context.Menus.FirstOrDefault(x => x.ID == menuID);

            if (menu == null)
                throw new ArgumentOutOfRangeException(nameof(menuID));

            List<Category> _categories = new();
            foreach(CategoryDTO category in itemDTO.Categories)
            {
                Category? _category = menu.Categories.FirstOrDefault(x => x.ID == category.ID);

                if (_category != null)
                    _categories.Add(_category);
            }
            

            menu.Items.Add(new Item(itemDTO) { Categories = _categories });
            _context.SaveChanges();

            return itemDTO.ID;
        }

        public ItemDTO? Get(int menuID, int itemID)
        {
            Item? item = _context.Items.Include(x => x.Categories).Include(x => x.Menu).FirstOrDefault(x => x.MenuID == menuID && x.ID == itemID);
            if (item == null)
                return null;

            return item.ToDTO(true);
        }
        
        public bool Update(int menuID, ItemDTO itemDTO)
        {
            Menu? menu = _context.Menus
                .Include(x => x.Categories)
                .Include(x => x.Items)
                .FirstOrDefault(x => x.ID == menuID);

            if (menu == null)
                return false;

            Item? item = menu.Items.FirstOrDefault(x => x.ID == itemDTO.ID);

            if (item == null)
                return false;

            item.Price = itemDTO.Price;
            item.Description = itemDTO.Description;
            item.Name = itemDTO.Name;

            string _tags = "";
            foreach(string tag in itemDTO.Tags)
            {                
                _tags += tag + " ";
            }
            item.Tags = _tags.Trim();

            List<Category> _categories = new();
            foreach (CategoryDTO category in itemDTO.Categories)
            {
                Category? _category = menu.Categories.FirstOrDefault(x => x.ID == category.ID);

                if (_category != null)
                    _categories.Add(_category);
            }
            item.Categories = _categories;

            _context.Items.Update(item);
            return _context.SaveChanges() > 0;
        }

        public bool Archive(int menuID, int itemID, bool restore = false)
        {
            Item? item = _context.Items.FirstOrDefault(x => x.MenuID == menuID && x.ID == itemID);
            if (item == null)
                return false;

            item.Archived = !restore;
            return _context.SaveChanges() > 0;
        }

        public List<ItemDTO> GetAll(int menuID)
        {
            Menu? menu = _context.Menus.Include(x => x.Items).FirstOrDefault(x => x.ID == menuID);

            if (menu == null)
                return null;

            List<ItemDTO> _items = new();
            foreach(Item item in menu.Items)
            {
                _items.Add(item.ToDTO());
            }

            return _items;
        }
    }
}

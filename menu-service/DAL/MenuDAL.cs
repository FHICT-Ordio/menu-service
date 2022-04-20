﻿using Microsoft.EntityFrameworkCore;

using AL;
using DTO;
using DAL.Model;


namespace DAL
{
    public class MenuDAL : IMenuCollection
    {
        private readonly MenuContext _context;

        public MenuDAL(DbContext context)
        {
            _context = context as MenuContext ?? throw new ArgumentNullException(nameof(context));
        }

        public int Add(MenuDTO menuDTO)
        {
            if (menuDTO == null)
                throw new ArgumentNullException(nameof(menuDTO));


            _context.Menus.Add(new Menu(menuDTO) { LastEdited = DateTime.Now });
            _context.SaveChanges();

            return menuDTO.ID;
        }

        public MenuDTO? Get(int ID)
        {
            Menu? menu = _context.Menus
                .Include(x => x.Items)
                .ThenInclude(x => x.Categories)
                .Include(x => x.Categories)
                .ThenInclude(x => x.Items)
                .FirstOrDefault(x => x.ID == ID);

            if (menu == null)
                return null;

            return menu.ToDTO(true, false, true);
        }

        public bool Update(MenuDTO menuDTO)
        {
            Menu? menu = _context.Menus.FirstOrDefault(x => x.ID == menuDTO.ID);

            if (menu == null)
                return false;

            menu.Title = menuDTO.Title;
            menu.Description = menuDTO.Description;
            menu.RestaurantName = menuDTO.RestaurantName;                       
            menu.LastEdited = DateTime.Now;
            
            _context.Menus.Update(menu);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int ID)
        {
            Menu? menu = _context.Menus.Include(x => x.Items).Include(x => x.Categories).FirstOrDefault(x => x.ID == ID);
            if (menu == null)
                return false;

            _context.Menus.Remove(menu);
            return _context.SaveChanges() > 0;
        }
    }
}

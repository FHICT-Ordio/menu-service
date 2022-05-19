using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;

namespace AL
{
    public interface IMenuCollection
    {
        public int Add(MenuDTO menuDTO);
        public MenuDTO? Get(int ID);
        public bool Update(MenuDTO menuDTO);
        public bool Archive(int ID, bool restore = false);

        public List<MenuDTO> GetAll(string owerID, bool getArchived);
    }
}

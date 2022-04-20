using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;

namespace AL
{
    public interface IItemCollection
    {
        public int Add(int menuID, ItemDTO itemDTO);
        public ItemDTO? Get(int menuID, int itemID);
        public bool Update(int menuID, ItemDTO itemDTO);
        public bool Delete(int menuID, int itemID);

        public List<ItemDTO> GetAll(int menuID);
    }
}

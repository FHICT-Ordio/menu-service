using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTO;

namespace AL
{
    public interface ICategoryCollection
    {
        public int Add(int menuID, CategoryDTO categoryDTO);
        public CategoryDTO? Get(int menuID, int categoryID);
        public bool Update(int menuID, CategoryDTO categoryDTO);
        public bool Delete(int menuID, int categoryID);
    }
}

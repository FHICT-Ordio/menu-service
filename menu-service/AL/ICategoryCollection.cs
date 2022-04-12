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
        public DTO.Category? Get(int id);
        public int Add(DTO.Category item, int menuId);
        public bool Delete(int id);
    }
}

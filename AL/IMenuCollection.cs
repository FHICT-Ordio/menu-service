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
        public Menu? Get(int id);
        public int Add(Menu item);
        public bool Delete(int id);
    }
}

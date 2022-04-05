using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class CategoryItem
    {
        // Foreign Keys
        public int CategoryID { get; set; }
        public int ItemID { get; set; }

        // Navigational Properties
        public Category Category { get; set; }
        public Item Item { get; set; }
    }
}

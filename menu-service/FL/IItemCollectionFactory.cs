using Microsoft.EntityFrameworkCore;

using AL;
using DAL;


namespace FL
{
    public static class IItemCollectionFactory
    {
        public static IItemCollection Get(DbContext context)
        {
            return new ItemDAL(context);
        }
    }
}

using Microsoft.EntityFrameworkCore;

using AL;
using DAL;


namespace FL
{
    public static class IMenuCollectionFactory
    {
        public static IMenuCollection Get(DbContext context)
        {
            return new MenuDAL(context);
        }
    }
}

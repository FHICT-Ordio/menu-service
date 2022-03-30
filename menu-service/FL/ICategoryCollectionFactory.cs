using Microsoft.EntityFrameworkCore;

using AL;
using DAL;


namespace FL
{
    public static class ICategoryCollectionFactory
    {
        public static ICategoryCollection Get(DbContext context)
        {
            return new CategoryDAL(context);
        }
    }
}

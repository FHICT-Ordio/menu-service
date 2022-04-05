#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;

using DTO;

namespace DAL
{
    public class MenuContext: DbContext
    {
        public MenuContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Menu> Menus { get; set; }                
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<CategoryItem> CategoryItems { get; set; }
        public DbSet<ItemImage> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<Menu>().ToTable("Menus");            
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<CategoryItem>().ToTable("CategoryItems");

            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Categories)
                .WithOne(c => c.Menu);
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Items)
                .WithOne(i => i.Menu);
            modelBuilder.Entity<Category>()
                .HasMany(c => c.CategoryItems)
                .WithOne(ci => ci.Category);
            modelBuilder.Entity<Item>()
                .HasMany(i => i.CategoryItems)
                .WithOne(ci => ci.Item)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CategoryItem>()
                    .HasKey(c => new { c.CategoryID, c.ItemID });

            modelBuilder.Entity<ItemImage>().ToTable("Images").HasNoKey();
        }
    }
}

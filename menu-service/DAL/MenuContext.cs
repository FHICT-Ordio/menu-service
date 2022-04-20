#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;

using DAL.Model;

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
        public DbSet<ItemImage> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<Menu>().ToTable("Menus");            
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Item>().ToTable("Items");

            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Categories)
                .WithOne(c => c.Menu)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Items)
                .WithOne(i => i.Menu)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>()
                .HasMany(x => x.Categories)
                .WithMany(x => x.Items);
            modelBuilder.Entity<Category>()
                .HasMany(x => x.Items)
                .WithMany(x => x.Categories);            

            modelBuilder.Entity<ItemImage>().ToTable("Images").HasNoKey();
        }
    }
}

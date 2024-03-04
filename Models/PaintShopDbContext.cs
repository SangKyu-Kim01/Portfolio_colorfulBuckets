using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace PaintShopManagement.Models
{
    public partial class PaintShopDbContext : DbContext
    {
        public PaintShopDbContext()
            : base("name=PaintShopDbContext")
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Orders> Orders { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>()
                .Property(i => i.price)
                .HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}

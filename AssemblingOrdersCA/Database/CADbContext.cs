using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AssemblingOrdersCA
{
    public sealed class CADbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrder { get; set; }
        public DbSet<ProductShelf> ProductShelf { get; set; }

        public CADbContext(DbContextOptions<CADbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductOrder>()
            .HasKey(po => new { po.ProductID, po.OrderID });

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOrder)
                .HasForeignKey(po => po.ProductID);

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Order)
                .WithMany(o => o.ProductOrder)
                .HasForeignKey(po => po.OrderID);

            modelBuilder.Entity<ProductShelf>()
                .HasKey(ps => new { ps.ProductID, ps.ShelfID });

            modelBuilder.Entity<ProductShelf>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductShelf)
                .HasForeignKey(ps => ps.ProductID);

            modelBuilder.Entity<ProductShelf>()
                .HasOne(ps => ps.Shelf)
                .WithMany(s => s.ProductShelf)
                .HasForeignKey(ps => ps.ShelfID);
        }
    }
}

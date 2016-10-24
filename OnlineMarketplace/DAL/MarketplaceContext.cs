using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.DAL
{
    public class MarketplaceContext : DbContext
    {
        public MarketplaceContext() : base("MarketplaceContext")
        {
            
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Store>()
                .HasMany(c => c.Products).WithMany(i => i.Stores)
                .Map(t => t.MapLeftKey("StoreID")
                    .MapRightKey("ProductID")
                    .ToTable("StoreProduct"));
        }
    }
}
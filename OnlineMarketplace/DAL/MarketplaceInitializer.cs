using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineMarketplace.Models;

namespace OnlineMarketplace.DAL
{
    public class MarketplaceInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<MarketplaceContext>
    {
        protected override void Seed(MarketplaceContext context)
        {
            var products = new List<Product>
            {
                new Product {ProductId = 1, Name = "Milk", Description = "2,5%", Stores = new List<Store>()},
                new Product {ProductId = 2, Name = "Bread", Description = "Baked on 22/Oct/2016", Stores = new List<Store>()},
                new Product {ProductId = 3, Name = "Iron", Description = "Made in Belarus", Stores = new List<Store>()},
                new Product {ProductId = 4, Name = "Hairdrier", Description = "Made in China", Stores = new List<Store>()},
                new Product {ProductId = 5, Name = "Laptop", Description = "Dell", Stores = new List<Store>()},
                new Product {ProductId = 6, Name = "Smarthpone", Description = "Apple", Stores = new List<Store>()}
            };

            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();

            var stores = new List<Store>
            {
                new Store {StoreId = 1, Title = "Euroopt", OpenTime = TimeSpan.Parse("9:00:00"), CloseTime = TimeSpan.Parse("22:00:00"), Products = new List<Product>()},
                new Store {StoreId = 2, Title = "21vek", OpenTime = TimeSpan.Parse("9:00:00"), CloseTime = TimeSpan.Parse("20:00:00"), Products = new List<Product>() }
            };

            stores.ForEach(s => context.Stores.Add(s));
            context.SaveChanges();

            AddOrUpdateProduct(context, "Euroopt", "Milk");
            AddOrUpdateProduct(context, "Euroopt", "Bread");
            AddOrUpdateProduct(context, "Euroopt", "Iron");
            AddOrUpdateProduct(context, "21vek", "Iron");
            AddOrUpdateProduct(context, "21vek", "Hairdrier");
            AddOrUpdateProduct(context, "21vek", "Laptop");
            AddOrUpdateProduct(context, "21vek", "Smarthpone");

            context.SaveChanges();
        }

        void AddOrUpdateProduct(MarketplaceContext context, string storeTitle, string productName)
        {
            var store = context.Stores.SingleOrDefault(s => s.Title == storeTitle);
            if (store != null)
            {
                var product = store.Products.SingleOrDefault(p => p.Name == productName);
                if (product == null)
                {
                    store.Products.Add(context.Products.Single(p => p.Name == productName));
                }
            }
        }
    }
}
using Kaede.DbContexts;
using Kaede.DTOs;
using Kaede.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Test.Models
{
    public class ShopItemTest
    {
        [Fact]
        public void CreateShopItem_ShouldPassUnlessDuplicateName()
        {
            using var context = new KaedeDbContext(Shared.CreateSqliteInMemOptions<KaedeDbContext>());
            context.Database.EnsureCreated();

            ShopItem item1 = new ShopItem
            {
                Name = "I1",
                Description = "Some random item",
                Price = 6.66M, 
                Duration = TimeSpan.FromMinutes(15)
            };

            ShopItem item2 = new ShopItem
            {
                Name = "I2",
                Description = null,
                Price = 8M,
                Duration = TimeSpan.FromMinutes(30)
            };

            context.ShopItems.Add(item1);
            context.ShopItems.Add(item2);
            context.SaveChanges();

            var shopItemList = context.ShopItems.ToList();
            Assert.NotNull(shopItemList.Find(i => i.Price.Equals(6.66M)));
            Assert.NotNull(shopItemList.Find(i => i.Name == "I2" && 
                i.Duration ==  TimeSpan.FromMinutes(30)));

            ShopItem item3 = new ShopItem
            {
                Name = "I1",
                Description = null,
                Price = 4.12M,
                Duration = TimeSpan.FromMinutes(5)
            };

            context.ShopItems.Add(item3);
            Assert.Throws<DbUpdateException>(() => context.SaveChanges());
        }
    }
}

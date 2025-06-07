using Kaede.DbContexts;
using Kaede.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaede.Models;
using Kaede.Extensions;

namespace Kaede.Services.ShopItemService
{
    public class DatabaseShopItemService : IShopItemService
    {
        IDbContextFactory<KaedeDbContext> _dbContextFactory;

        public DatabaseShopItemService(IDbContextFactory<KaedeDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task CreateShopItem(ShopItemDTO shopItemDTO)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            await context.ShopItems.AddAsync(ShopItem.FromDTO(shopItemDTO));
            await context.SaveChangesAsync();
        }

        public async Task<ShopItemDTO?> GetShopItemByName(string name)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var item = await context.ShopItems.FirstOrDefaultAsync(i => i.Name == name);
            return item?.MapToDTO();
        }

        public async Task<List<ShopItemDTO>> GetAllShopItems()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            return await context.ShopItems.Select(i => i.MapToDTO()).ToListAsync();
        }
    }
}

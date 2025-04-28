using Kaede.DbContexts;
using Kaede.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }

        public async Task GetShopItemByName(string name)
        {

        }

        public Task<List<ShopItemDTO>> GetAllShopItems()
        {
            throw new NotImplementedException();
        }
    }
}

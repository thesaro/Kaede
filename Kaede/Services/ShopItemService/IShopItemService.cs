using Kaede.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Services.ShopItemService
{
    public interface IShopItemService
    {
        Task CreateShopItem(ShopItemDTO shopItemDTO);
        Task<ShopItemDTO?> GetShopItemByName(string name);
        Task<List<ShopItemDTO>> GetAllShopItems();
    }
}

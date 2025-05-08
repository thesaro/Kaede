using CommunityToolkit.Mvvm.Input;
using Kaede.DTOs;
using Kaede.Services;
using Kaede.Services.ShopItemService;
using Kaede.Services.UsersService;
using Kaede.Stores;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Windows;

namespace Kaede.ViewModels
{
    public class AppointmentListingViewModel : ViewModelBase
    {

    }

    public class ShopItemSubmitionViewModel : ViewModelBase
    {
        #region Services & Dependencies
        private readonly ILogger<ShopItemSubmitionViewModel> _logger;
        private readonly IShopItemService _shopItemService;
        #endregion

        #region Commands
        IRelayCommand SubmitCommand { get; }
        #endregion


        #region Properties
        private string _name = "";
        [Required]
        [Length(5, 50)]
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        private string _description = "";
        [Required]
        [Length(0, 100)]
        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        private decimal? _price = null;
        [Required]
        public string Price
        {
            get => _price.ToString() ?? "";
            set
            {
                if (string.IsNullOrEmpty(value))
                    SetProperty(ref _price, null);
                else if (decimal.TryParse(value, out decimal p))
                {
                    SetProperty(ref _price, p);
                    SubmitCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private int? _hours = 0;
        [Required]
        public string Hours
        {
            get => _hours.ToString() ?? "";
            set
            {
                if (string.IsNullOrEmpty(value))
                    SetProperty(ref _hours, null);
                else if (int.TryParse(value, out int h))
                {
                    SetProperty(ref _hours, h);
                    SubmitCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private int? _minutes = 0;
        [Required]
        public string Minutes
        {
            get => _minutes.ToString() ?? "";
            set
            {
                if (string.IsNullOrEmpty(value))
                    SetProperty(ref _minutes, null);
                else if (int.TryParse(value, out int m))
                {
                    SetProperty(ref _minutes, m);
                    SubmitCommand.NotifyCanExecuteChanged();
                }
            }
        }
        #endregion


        #region Constructor
        public ShopItemSubmitionViewModel(
            ILogger<ShopItemSubmitionViewModel> logger,
            IShopItemService shopItemService)
        {
            _logger = logger;
            _shopItemService = shopItemService;

            SubmitCommand = new AsyncRelayCommand(SubmitShopItem, CanSubmitShopItem);
            SubmitCommand.NotifyCanExecuteChanged();
        }
        #endregion

        #region SubmitShopItemMethods

        private async Task SubmitShopItem()
        {
            // TODO: check if item already exists

            if (_price == null || _hours == null || _minutes == null)
            {
                _logger.LogError("Price is null at SubmitShopItem().");
                return;
            }
            
            int totalMinutes = _hours.Value * 60 + _minutes.Value;

            ShopItemDTO shopItemDTO = new ShopItemDTO()
            {
                Name = this.Name,
                Description = this.Description,
                Price = (decimal)_price,
                Duration = TimeSpan.FromMinutes(totalMinutes)
            };

            try
            {
                await _shopItemService.CreateShopItem(shopItemDTO);
       
                MessageBox.Show($"Shop Item \"{shopItemDTO.Name}\" successfully registered", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register item.\nReason: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearErrors();
            SubmitCommand.NotifyCanExecuteChanged();
        }

        private bool CanSubmitShopItem() =>
            !HasErrors &&
            !string.IsNullOrEmpty(Name) &&
            !string.IsNullOrEmpty(Description) &&
            _price > 0M && !(_hours == 0 && _minutes == 0);
        #endregion
    }


    public class DashboardViewModel : ViewModelBase
    {
        #region Child ViewModels
        public ShopItemSubmitionViewModel ShopItemSubmitionVM { get; }
        #endregion


        public DashboardViewModel(ShopItemSubmitionViewModel shopItemSubmitionVM)
        {
            ShopItemSubmitionVM = shopItemSubmitionVM;
        }
    }
}

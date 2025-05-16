using CommunityToolkit.Mvvm.Input;
using Kaede.DTOs;
using Kaede.Services;
using Kaede.Services.AppointmentsService;
using Kaede.Services.ShopItemService;
using Kaede.Services.UsersService;
using Kaede.Stores;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Kaede.ViewModels
{
    public class AppointmentSubmitionViewModel : ViewModelBase
    {
        #region Services & Dependencies
        private readonly ILogger<AppointmentSubmitionViewModel> _logger;
        private readonly IUserService _userService;
        private readonly IShopItemService _shopItemService;
        private readonly IAppointmentService _appointmentService;
        #endregion

        #region Commands
        public IRelayCommand SubmitAppointmentCommand { get; }
        public IRelayCommand AddCustomerCommand { get; }
        #endregion

        #region Properties

        private ObservableCollection<CustomerDTO> _customers;
        public ListCollectionView FilteredCustomers { get; set; }

        private CustomerDTO _selectedCustomer;
        public CustomerDTO SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        private string _customerSearchText = string.Empty;
        public string CustomerSearchText
        {
            get => _customerSearchText;
            set
            {
                SetProperty(ref _customerSearchText, value);
                FilteredCustomers.Refresh();
                ShowAddCustomerButton = FilteredCustomers.IsEmpty 
                    && !string.IsNullOrEmpty(value);
            }
        }

        public bool _showAddCustomerButton = false; 
        public bool ShowAddCustomerButton
        {
            get => _showAddCustomerButton;
            set => SetProperty(ref _showAddCustomerButton, value);
        }

        private ObservableCollection<UserDTO> _barbers;
        public ListCollectionView FilteredBarbers { get; set; }

        private UserDTO _selectedBarber;
        public UserDTO SelectedBarber
        {
            get => _selectedBarber;
            set => SetProperty(ref _selectedBarber, value);
        }

        private string _barberSearchText = string.Empty;
        public string BarberSearchText
        {
            get => _barberSearchText;
            set
            {
                SetProperty(ref _barberSearchText, value);
                FilteredBarbers.Refresh();
            }
        }

        private ObservableCollection<ShopItemDTO> _shopItems;
        public ListCollectionView FilteredShopItems { get; set; }

        private ShopItemDTO _selectedShopItem;
        public ShopItemDTO SelectedShopItem
        {
            get => _selectedShopItem;
            set
            {
                SetProperty(ref _selectedShopItem, value);
                // TODO: if the start date is not empty, auto set end date
                // based on the selected shop item
            }
        }

        private string _shopItemSearchText = string.Empty;
        public string ShopItemSearchText
        {
            get => _shopItemSearchText;
            set
            {
                SetProperty(ref _shopItemSearchText, value);
                FilteredShopItems.Refresh();
            }
        }
        #endregion

        private DateTime _startTime = DateTime.Now; 
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                // TODO: that uhh auto thingy
                SetProperty(ref _startTime, value);
            }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        #region Constructor
        public AppointmentSubmitionViewModel(
            ILogger<AppointmentSubmitionViewModel> logger,
            IUserService userService,
            IShopItemService shopItemService,
            IAppointmentService appointmentService
            )
        {
            _logger = logger;
            _userService = userService;
            _shopItemService = shopItemService;
            _appointmentService = appointmentService;

            Task.WhenAll(
                FetchCustomers(), 
                FetchBarbers(),
                FetchShopItems())
                .GetAwaiter().GetResult();

            FilteredCustomers = new ListCollectionView(_customers);
            FilteredCustomers.Filter = item =>
            {
                if (string.IsNullOrEmpty(CustomerSearchText)) return true;
                return ((CustomerDTO)item).FullName.IndexOf(CustomerSearchText, StringComparison.Ordinal) >= 0;
            };

            AddCustomerCommand = new AsyncRelayCommand(SubmitCustomer);
        }
        #endregion


        #region Methods

        private async Task FetchCustomers()
        {
            _customers = new ObservableCollection<CustomerDTO>
                (await _appointmentService.GetAllCustomers());
            FilteredCustomers = new ListCollectionView(_customers);
            FilteredCustomers.Filter = item =>
            {
                if (string.IsNullOrEmpty(CustomerSearchText)) return true;
                return ((CustomerDTO)item).FullName.IndexOf(CustomerSearchText, StringComparison.Ordinal) >= 0;
            };

            _logger.LogDebug("Customers collection updated with values: {CustomersList}", _customers);
        }

        private async Task FetchBarbers()
        {
            _barbers = new ObservableCollection<UserDTO>
                (await _userService.GetAllBarbers());
            FilteredBarbers = new ListCollectionView(_barbers);
            FilteredBarbers.Filter = item =>
            {
                if (string.IsNullOrEmpty(BarberSearchText)) return true;
                return ((UserDTO)item).Username.IndexOf(BarberSearchText, StringComparison.Ordinal) >= 0;
            };

            _logger.LogDebug("Barbers collection updated with values {BarbersList}", _barbers);
        }

        private async Task FetchShopItems()
        {
            _shopItems = new ObservableCollection<ShopItemDTO>
                (await _shopItemService.GetAllShopItems());
            FilteredShopItems = new ListCollectionView(_shopItems);
            FilteredShopItems.Filter = item =>
            {
                if (string.IsNullOrEmpty(ShopItemSearchText)) return true;
                return ((ShopItemDTO)item).Name.IndexOf(ShopItemSearchText, StringComparison.Ordinal) >= 0;
            };
        }

        private async Task SubmitCustomer()
        {
            // Avoid quick textfield UI changes to affect this operation 
            var customerName = new string (CustomerSearchText);

            CustomerDTO customerDTO = new CustomerDTO
            { 
                FullName = customerName,
                PhoneNumber = null
            };

            try
            {
                _logger.LogInformation("Attempting to register customer : {FullName}", customerDTO.FullName);

                await _appointmentService.CreateCustomer(customerDTO);
                MessageBox.Show($"Customer \"{customerDTO.FullName}\" successfully registered.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                _logger.LogInformation("Customer {FullName} successfully registered", customerDTO.FullName);


                CustomerSearchText = string.Empty;
                ShowAddCustomerButton = false;
                await FetchCustomers();
                FilteredCustomers.Refresh();
            }
            catch
            {
                _logger.LogError("Unable to create new customer.");
            }
        }

        private async Task SubmitAppointment()
        {

        }
        #endregion
    }
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
        public IRelayCommand SubmitItemCommand { get; }
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
                SubmitItemCommand.NotifyCanExecuteChanged();
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
                SubmitItemCommand.NotifyCanExecuteChanged();
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
                    SubmitItemCommand.NotifyCanExecuteChanged();
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
                    SubmitItemCommand.NotifyCanExecuteChanged();
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
                    SubmitItemCommand.NotifyCanExecuteChanged();
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

            SubmitItemCommand = new AsyncRelayCommand(SubmitShopItem, CanSubmitShopItem);
            SubmitItemCommand.NotifyCanExecuteChanged();
        }
        #endregion

        #region SubmitShopItemMethods

        private async Task SubmitShopItem()
        {
            if ((await _shopItemService.GetShopItemByName(Name)) != null)
            {
                MessageBox.Show($"Shop Item \"{Name}\" already exists.", "Error",
                    MessageBoxButton.OK);
                return;
            }

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
                OnShopItemAdded((await _shopItemService.GetShopItemByName(shopItemDTO.Name))!);
                MessageBox.Show($"Shop Item \"{shopItemDTO.Name}\" successfully registered", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register item.\nReason: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearErrors();
            SubmitItemCommand.NotifyCanExecuteChanged();
        }

        private bool CanSubmitShopItem() =>
            !HasErrors &&
            !string.IsNullOrEmpty(Name) &&
            !string.IsNullOrEmpty(Description) &&
            _price > 0M && !(_hours == 0 && _minutes == 0);
        #endregion

        #region Events

        public Action<ShopItemDTO>? ShopItemAdded;

        private void OnShopItemAdded(ShopItemDTO item)
        {
            ShopItemAdded?.Invoke(item);
        }
        #endregion
    }

    public class ShopItemListingViewModel : ViewModelBase
    {
        #region Services & Dependencies
        private readonly IShopItemService _shopItemService;
        #endregion

        #region Commands
        public ICommand RemoveItemCommand { get; }
        #endregion

        #region Properties
        private readonly ObservableCollection<ShopItemDTO> _shopItems;
        public IEnumerable<ShopItemDTO> ShopItems => _shopItems;
        #endregion

        #region Constructor
        public ShopItemListingViewModel(IShopItemService shopItemService)
        {
            // TODO: Remove shop item command
            _shopItemService = shopItemService;
            List<ShopItemDTO> res = _shopItemService.GetAllShopItems().GetAwaiter().GetResult();
            _shopItems = new ObservableCollection<ShopItemDTO>(res);
        }
        #endregion

        #region Methods
        public void AddShopItem(ShopItemDTO dto)
        {
            _shopItems.Add(dto);
        }
        #endregion
    }

    public class DashboardViewModel : ViewModelBase
    {
        #region Child ViewModels
        public ShopItemSubmitionViewModel ShopItemSubmitionVM { get; }
        public ShopItemListingViewModel ShopItemListingVM { get; }
        public AppointmentSubmitionViewModel AppointmentSubmitionVM { get; }
        #endregion


        public DashboardViewModel(
            ShopItemSubmitionViewModel shopItemSubmitionVM, 
            ShopItemListingViewModel shopItemListingVM,
            AppointmentSubmitionViewModel appointmentSubmitionVM)
        {
            ShopItemSubmitionVM = shopItemSubmitionVM;
            ShopItemListingVM = shopItemListingVM;
            AppointmentSubmitionVM = appointmentSubmitionVM;

            ShopItemSubmitionVM.ShopItemAdded += ShopItemListingVM.AddShopItem;
        }
    }
}

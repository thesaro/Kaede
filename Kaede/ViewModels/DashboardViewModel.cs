using CommunityToolkit.Mvvm.Input;
using Kaede.DTOs;
using Kaede.Services;
using Kaede.Services.AppointmentsService;
using Kaede.Services.ShopItemService;
using Kaede.Services.UsersService;
using Kaede.Stores;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Kaede.Models;
using System.Linq.Expressions;
using Expression = System.Linq.Expressions.Expression;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.ComponentModel;
using Kaede.Bindables;

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
                // if the start date is not empty, auto set end date
                // based on the selected shop item
                EndTime = StartTime + _selectedShopItem.Duration;
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

        private DateTime _startTime = DateTime.Now;
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                SetProperty(ref _startTime, value);
                EndTime = _startTime + SelectedShopItem.Duration;
            }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }
        #endregion



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
            SubmitAppointmentCommand = new AsyncRelayCommand(SubmitAppointment, CanSubmitAppointment);
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

            if (!string.IsNullOrEmpty(customerName) && !System.Text.RegularExpressions.Regex.IsMatch(customerName, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Customer name can only contain letters and spaces.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            AppointmentDTO appointmentDTO = new AppointmentDTO
            {
                CustomerDTO = SelectedCustomer,
                BarberDTO = SelectedBarber,
                ShopItemDTO = SelectedShopItem,
                StartDate = StartTime,
                EndDate = EndTime,
                Status = AppointmentStatus.Pending
            };

            if (EndTime <= StartTime)
            {
                MessageBox.Show("End time must be after start time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _logger.LogInformation("Attempting to submit appointment: {AppointmentDTO}", appointmentDTO.ToString());

                appointmentDTO.AppointmentId = await _appointmentService.CreateAppointment(appointmentDTO);
                
                _logger.LogInformation("Appointment {AppointmentDTO} successfully registered", appointmentDTO.ToString());

                AppointmentSubmitted.Value = appointmentDTO;

                MessageBox.Show($"Appointment successfully registered", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Failed to create appointment based on {AppointmentDTO}",
                    appointmentDTO, ex);
            }
        }

        private bool CanSubmitAppointment()
        {
            // TODO: Actually implement this logic
            return true;
        }

        #endregion

        #region Events
        public Bindable<AppointmentDTO> AppointmentSubmitted { get; } = new();
        #endregion
    }
    public class AppointmentListingViewModel : ViewModelBase
    {
        #region Services & Dependencies
        private readonly ILogger<AppointmentListingViewModel> _logger;
        private readonly UserSession _userSession;
        private readonly IAppointmentService _appointmentService;
        #endregion

        #region Commands
        public IRelayCommand CancelAppointmentCommand { get; }
        public IRelayCommand MarkAppointmentDoneCommand { get; }
        #endregion

        #region AppointmentFilterManager
        private enum AppointmentFilter
        {
            StatusPending,
            StatusConfirmed
        }

        private class AppointmentFilterManager
        {
            private Dictionary<AppointmentFilter, Expression<Func<AppointmentDTO, bool>>> _filterMap =
                new()
                {
                    {AppointmentFilter.StatusConfirmed, a => a.Status == AppointmentStatus.Canceled || a.Status == AppointmentStatus.Done },
                    {AppointmentFilter.StatusPending, a => a.Status == AppointmentStatus.Pending },
                };

            private List<AppointmentFilter> _activeFilters = new();

            public event EventHandler? ActiveFilterChanged;
            public void RaiseActiveFiltersChanged()
            {
                ActiveFilterChanged?.Invoke(this, EventArgs.Empty);
            }

            public void ApplyFilter(AppointmentFilter filter, bool raiseUpdates = true)
            {
                if (!_activeFilters.Contains(filter))
                    _activeFilters.Add(filter);
                if (raiseUpdates)
                    RaiseActiveFiltersChanged();
            }

            public void RemoveFilter(AppointmentFilter filter, bool raiseUpdates = true)
            {
                _activeFilters.Remove(filter);
                if (raiseUpdates)
                    RaiseActiveFiltersChanged();
            }

            public Expression<Func<AppointmentDTO, bool>> GetCombinedFilter()
            {
                if (!_activeFilters.Any()) return a => true; // No filters applied, return all

                var parameter = Expression.Parameter(typeof(AppointmentDTO), "a");
                Expression combinedExpression = Expression.Constant(true);

                foreach (var filter in _activeFilters)
                {
                    var expression = _filterMap[filter].Body;
                    combinedExpression = Expression.AndAlso(combinedExpression, Expression.Invoke(_filterMap[filter], parameter));
                }

                return Expression.Lambda<Func<AppointmentDTO, bool>>(combinedExpression, parameter);
            }

        }
        #endregion

        #region Properties

        private readonly AppointmentFilterManager _appointmentFilterManager;
        private ObservableCollection<AppointmentDTO> _appointments;
        public ObservableCollection<AppointmentDTO> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        private ICollectionView _appointmentsFinalized;
        public ICollectionView AppointmentsFinalized
        {
            get => _appointmentsFinalized;
            set => SetProperty(ref _appointmentsFinalized, value);
        }


        private bool _isInactiveAPsToggled = false;
        public bool IsInactiveAPsToggled
        {
            get => _isInactiveAPsToggled;
            set
            {
                SetProperty(ref _isInactiveAPsToggled, value);

                if (_isInactiveAPsToggled)
                {
                    _appointmentFilterManager.RemoveFilter(AppointmentFilter.StatusPending, raiseUpdates: false);
                    _appointmentFilterManager.ApplyFilter(AppointmentFilter.StatusConfirmed);

                    IsCancelColumnVisible = IsMarkDoneColumnVisible = false;
                }
                else
                {
                    _appointmentFilterManager.RemoveFilter(AppointmentFilter.StatusConfirmed, raiseUpdates: false);
                    _appointmentFilterManager.ApplyFilter(AppointmentFilter.StatusPending);

                    IsCancelColumnVisible = IsMarkDoneColumnVisible = true;
                }
            }
        }

        private bool _isCancelColumnVisible = true;
        public bool IsCancelColumnVisible
        {
            get => _isCancelColumnVisible;
            set => SetProperty(ref _isCancelColumnVisible, value);
        }

        private bool _isMarkDoneColumnVisible = true;
        public bool IsMarkDoneColumnVisible
        {
            get => _isMarkDoneColumnVisible;
            set => SetProperty(ref _isMarkDoneColumnVisible, value);
        }

        public enum AppointmentOrdering
        {
            StartDateAscending,
            StartDateDescending,
            BarberAlphabetic,
            ShopItemAlphabetic,
            ShopItemPriceDescending
        }

        public ObservableCollection<AppointmentOrdering> AppointmentOrderingOptions { get; }
            = new ObservableCollection<AppointmentOrdering>(
                Enum.GetValues(typeof(AppointmentOrdering))
                    .Cast<AppointmentOrdering>()
                    .ToList());

        private AppointmentOrdering _selectedappointmentOrdering = AppointmentOrdering.StartDateAscending;
        public AppointmentOrdering SelectedAppointmentOrdering
        {
            get => _selectedappointmentOrdering;
            set
            {
                SetProperty(ref _selectedappointmentOrdering, value);
                ReorderAppointments();
            }
        }
        #endregion

        #region Constructor
        public AppointmentListingViewModel(
            ILogger<AppointmentListingViewModel> logger,
            IAppointmentService appointmentService,
            UserSession userSession)
        {
            _logger = logger;
            _userSession = userSession;
            _appointmentService = appointmentService;

            _appointmentFilterManager = new AppointmentFilterManager();
            _appointmentFilterManager.ActiveFilterChanged += async (sender, e) => await FetchAppointments();

            // _appointments is initialized here
            _appointmentFilterManager.ApplyFilter(AppointmentFilter.StatusPending);


            CancelAppointmentCommand = new RelayCommand<object?>(CancelAppointment);
            MarkAppointmentDoneCommand = new RelayCommand<object?>(MarkAppointmentDone);
        }
        #endregion

        #region Methods

        private async Task FetchAppointments()
        {
            var combinedFilter = _appointmentFilterManager.GetCombinedFilter();
            var appointments = await _appointmentService.GetAllAppointments();
            var filteredAppointments = appointments.AsQueryable().Where(combinedFilter).ToList();
            Appointments = new ObservableCollection<AppointmentDTO>(filteredAppointments);
            AppointmentsFinalized = CollectionViewSource.GetDefaultView(Appointments);
            ReorderAppointments();
        }

        private void ReorderAppointments()
        {
            AppointmentsFinalized.SortDescriptions.Clear();
            switch (SelectedAppointmentOrdering)
            {
                case AppointmentOrdering.StartDateAscending:
                    AppointmentsFinalized.SortDescriptions.Add(
                        new SortDescription(nameof(AppointmentDTO.StartDate), ListSortDirection.Ascending));
                    break;
                case AppointmentOrdering.StartDateDescending:
                    AppointmentsFinalized.SortDescriptions.Add(
                        new SortDescription(nameof(AppointmentDTO.StartDate), ListSortDirection.Descending));
                    break;
                case AppointmentOrdering.BarberAlphabetic:
                    AppointmentsFinalized.SortDescriptions.Add(
                        new SortDescription(nameof(AppointmentDTO.BarberDTO.Username), ListSortDirection.Ascending));
                    break;
                case AppointmentOrdering.ShopItemAlphabetic:
                    AppointmentsFinalized.SortDescriptions.Add(
                        new SortDescription(nameof(AppointmentDTO.ShopItemDTO.Name), ListSortDirection.Ascending));
                    break;
                case AppointmentOrdering.ShopItemPriceDescending:
                    AppointmentsFinalized.SortDescriptions.Add(
                        new SortDescription(nameof(AppointmentDTO.ShopItemDTO.Price), ListSortDirection.Descending));
                    break;
                default:
                    break;
            }
            AppointmentsFinalized.Refresh();
        }

        private bool CanCurrentUserModifyAppointment(AppointmentDTO appointmentDTO) =>
            _userSession.CurrentUser!.Role == UserRole.Admin ||
                (_userSession.CurrentUser!.Role == UserRole.Barber &&
                appointmentDTO.BarberDTO.Username == _userSession.CurrentUser!.Username);

        public void CancelAppointment(object? item)
        {
            if (item != null && item is AppointmentDTO appointmentDTO)
            {

                if (!CanCurrentUserModifyAppointment(appointmentDTO))
                {
                    MessageBox.Show($"This appointment can be either cancelled by admin or {appointmentDTO.BarberDTO.Username}.",
                        "Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                
                MessageBoxResult removeRes = MessageBox.Show($"Do you really want to cancel this appointment?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (removeRes != MessageBoxResult.Yes)
                    return;

                try
                {
                    _logger.LogDebug("Attempting to cancel appointment {AppointmentDTO}", appointmentDTO);

                    _appointmentService.ChangeAppointmentStatus(appointmentDTO, AppointmentStatus.Canceled);
                    

                    _logger.LogDebug("Successfully canceled appointment {AppointmentDTO}, Updating appointments...", appointmentDTO);

                    FetchAppointments().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occured while cancelling appointment:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void MarkAppointmentDone(object? item)
        {
            if (item != null && item is AppointmentDTO appointmentDTO)
            {
                if (!CanCurrentUserModifyAppointment(appointmentDTO))
                {
                    MessageBox.Show($"This appointment can be marked as done either by admin or {appointmentDTO.BarberDTO.Username}.",
                        "Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                MessageBoxResult markDoneRes = MessageBox.Show($"Do you really want to mark this appointment as done?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (markDoneRes != MessageBoxResult.Yes)
                    return;

                try
                {
                    _appointmentService.ChangeAppointmentStatus(appointmentDTO, AppointmentStatus.Done);
                    FetchAppointments().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occured while marking appointment as done:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void AddAppointment(AppointmentDTO appointmentDTO)
        {
            Appointments.Add(appointmentDTO);
            AppointmentsFinalized.Refresh();
        }

        #endregion
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
                ShopItemAdded.Value = await _shopItemService.GetShopItemByName(shopItemDTO.Name)!;
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
        public Bindable<ShopItemDTO> ShopItemAdded { get; } = new();
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
        public AppointmentListingViewModel AppointmentListingVM { get; }
        #endregion


        public DashboardViewModel(
            ShopItemSubmitionViewModel shopItemSubmitionVM, 
            ShopItemListingViewModel shopItemListingVM,
            AppointmentSubmitionViewModel appointmentSubmitionVM,
            AppointmentListingViewModel appointmentListingVM)
        {
            ShopItemSubmitionVM = shopItemSubmitionVM;
            ShopItemListingVM = shopItemListingVM;
            AppointmentSubmitionVM = appointmentSubmitionVM;
            AppointmentListingVM = appointmentListingVM;

            ShopItemSubmitionVM.ShopItemAdded
                .BindValueChanged(e => ShopItemListingVM.AddShopItem(e.NewValue));
            AppointmentSubmitionVM.AppointmentSubmitted
                .BindValueChanged(e => AppointmentListingVM.AddAppointment(e.NewValue));
        }
    }
}

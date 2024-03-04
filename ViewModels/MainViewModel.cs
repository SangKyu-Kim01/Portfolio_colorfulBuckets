using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FontAwesome.Sharp;
using PaintShopManagement.Models;
using PaintShopManagement.Repositories;

namespace PaintShopManagement.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Fields
        private UserAccountModel _currentUserAccount;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;


        private IUserRepository userRepository;

        // Properties
        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }
            set
            {
                if (_currentUserAccount != value)
                {
                    _currentUserAccount = value;
                    OnPropertyChanged(nameof(CurrentUserAccount));
                    OnPropertyChanged(nameof(ProfilePicBlob));
                }
                Console.WriteLine($"Current User Name : {CurrentUserAccount.DisplayName} ");
                Console.WriteLine($"Current User photo : {CurrentUserAccount.ProfilePicture} ");
            }
        }

        public ImageSource ProfilePicBlob
        {
            get
            {
                if (CurrentUserAccount != null && CurrentUserAccount.ProfilePicture != null)
                {
                    try
                    {
                        // for debugging
                        Console.WriteLine($"ImageSource called - ProfilePicBlob : {CurrentUserAccount.ProfilePicture}");

                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new System.IO.MemoryStream(CurrentUserAccount.ProfilePicture);
                        image.EndInit();
                        return image;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating BitmapImage: {ex.Message}");
                        return null;
                    }
                }
                return null;
            }
        }

        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        // Commands
        public ICommand ShowDashboardViewCommand { get; }
        public ICommand ShowInventoryViewCommand { get; }
        public ICommand ShowCustomerViewCommand { get; }
        public ICommand ShowEmployeeViewCommand { get; }
        public ICommand ShowOrderViewCommand { get; }


        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();

            // Initialize Commands
            ShowDashboardViewCommand = new ViewModelCommand(ExecuteShowDashboardViewCommand);
            ShowInventoryViewCommand = new ViewModelCommand(ExecuteShowInventoryViewCommand);
            ShowCustomerViewCommand = new ViewModelCommand(ExecuteShowCustomerViewCommand, CanShowView);
            ShowEmployeeViewCommand = new ViewModelCommand(ExecuteShowEmployeeViewCommand, CanShowView);
            ShowOrderViewCommand = new ViewModelCommand(ExecuteShowOrderViewCommand, CanShowView);

            //Default View
            ExecuteShowDashboardViewCommand(null);

            LoadCurrentUserData();
        }

        private void ExecuteShowOrderViewCommand(object obj)
        {
            CurrentChildView = new OrderViewModel();
            Caption = "Order";
            Icon = IconChar.Truck;
        }

        private void ExecuteShowEmployeeViewCommand(object obj)
        {
            CurrentChildView = new EmployeeViewModel();
            Caption = "Employee";
            Icon = IconChar.AddressBook;
        }
        private void ExecuteShowCustomerViewCommand(object obj)
        {
            CurrentChildView = new CustomerViewModel();
            Caption = "Customer";
            Icon = IconChar.UserGroup;
        }

        private void ExecuteShowInventoryViewCommand(object obj)
        {
            CurrentChildView = new InventoryViewModel();
            Caption = "Inventory";
            Icon = IconChar.TableList;
        }

        private void ExecuteShowDashboardViewCommand(object obj)
        {
            CurrentChildView = new DashboardViewModel();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        // Access is allowed only for Manager
        private bool CanShowView(object obj)
        {
            // Extract position from GenericIdentity
            var positionString = Thread.CurrentPrincipal.Identity.Name.Split('|')[1];

            // Convert the position string to an integer
            if (int.TryParse(positionString, out int position))
            {
                // Check if the user has the required position to show the view
                return position == 1; // Admin : 1, Employee: 2
            }

            return false;
        }



        private void LoadCurrentUserData()
        {
            var _userName = Thread.CurrentPrincipal.Identity.Name.Split('|')[0];
            var user = userRepository.GetByUsername(_userName);
            if (user != null)
            {
                CurrentUserAccount.Username = user.Username;
                CurrentUserAccount.DisplayName = $"{user.Firstname} {user.Lastname}";
                CurrentUserAccount.ProfilePicture = user.ProfilePic;

                OnPropertyChanged(nameof(ProfilePicBlob));

                Console.WriteLine($"Current User Name set: {user.Username}");
                Console.WriteLine($"Current User FName set: {user.Firstname}");
                Console.WriteLine($"Current User Pic set: {user.ProfilePic}");
            }
            else
            {
                CurrentUserAccount.DisplayName = "User not logged in";

            }
        }

    }
}

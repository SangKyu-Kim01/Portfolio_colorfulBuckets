using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using PaintShopManagement.Views;

namespace PaintShopManagement.ViewModels
{
    internal class EditCustomerViewModel : ViewModelBase
    {
        private readonly Customers _customer;
        private readonly CustomerViewModel _customerViewModel;
        private readonly Window _currentWindow; 

        public EditCustomerViewModel(Customers customer, CustomerViewModel customerViewModel, Window currentWindow)
        {
            _customer = customer ?? throw new ArgumentNullException(nameof(customer));
            _customerViewModel = customerViewModel ?? throw new ArgumentNullException(nameof(customerViewModel));
            _currentWindow = currentWindow ?? throw new ArgumentNullException(nameof(currentWindow));

            InitializeProperties();
        }

        private void InitializeProperties()
        {
            FirstName = _customer.firstName;
            LastName = _customer.lastName;
            Company = _customer.company;
            Email = _customer.email;
            Phone = _customer.phone;
            Address = _customer.address;
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private string _company;
        public string Company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged(nameof(Company));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }


        public ICommand UpdateCustomerCommand => new RelayCommand(UpdateCustomer);
        public ICommand UpdateCloseCommand => new RelayCommand(UpdateClose);

        private void UpdateCustomer(object parameter)
        {
            try
            {
                _customer.firstName = FirstName;
                _customer.lastName = LastName;
                _customer.company = Company;
                _customer.email = Email;
                _customer.phone = Phone;
                _customer.address = Address;

                _customerViewModel.UpdateCustomer(_customer);
                _currentWindow?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating customer: {ex.Message}";
            }
        }

        private void UpdateClose(object parameter)
        {
            UpdateCustomer(parameter);
            _currentWindow?.Close();
        }

        public Window CurrentWindow { get; set; }
    }
}

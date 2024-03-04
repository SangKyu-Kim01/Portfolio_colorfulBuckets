using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using System.IO;
using System.Windows;
using System.Data.Entity;

namespace PaintShopManagement.ViewModels
{
    internal class AddCustomerViewModel : ViewModelBase
    {

        private readonly PaintShopDbContext _dbContext;

        // Constructor to initialize the PaintShopDbContext
        public AddCustomerViewModel(PaintShopDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            CreateCustomerCommand = new RelayCommand(CreateCustomer, CanCreateCustomer);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Fields
        private string _firstName;
        private string _lastName;
        private string _company;
        private string _email;
        private string _phone;
        private string _address;
        private string _errorMessage;

        // Properties 
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Company
        {
            get
            {
                return _company;
            }
            set
            {
                _company = value;
                OnPropertyChanged(nameof(Company));
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }



        // Commands
        public ICommand CreateCustomerCommand { get; }
        public ICommand CancelCommand { get; }

        public AddCustomerViewModel()
        {
            CreateCustomerCommand = new RelayCommand(CreateCustomer, CanCreateCustomer);
            CancelCommand = new RelayCommand(Cancel);
        }

        //Validation
        private bool CanCreateCustomer(object parameter)
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
          !string.IsNullOrWhiteSpace(LastName) &&
          !string.IsNullOrWhiteSpace(Company) &&
          !string.IsNullOrWhiteSpace(Address) &&
          IsValidEmail(Email) &&
          IsValidPhoneNumber(Phone);
        }
        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }

        private bool IsValidPhoneNumber(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && phone.All(char.IsDigit) && phone.Length >= 7;
        }
        //End of Validation

        private void CreateCustomer(object parameter)
        {
            // Check if all required fields are filled and if email and phone are valid
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Company) || string.IsNullOrWhiteSpace(Address) ||
                !IsValidEmail(Email) || !IsValidPhoneNumber(Phone))
            {
                ErrorMessage = "Please fill all required fields and provide valid email and phone.";
                return;
            }

            try
            {
                var customer = new Customers(FirstName, LastName, Company, Email, Phone, Address);

                if (_dbContext != null)
                {
                    _dbContext.Customers.Add(customer);
                    _dbContext.SaveChanges();
                    // Optional
                    ErrorMessage = "Customer created successfully.";
                }
                else
                {
                    ErrorMessage = "Database context is not initialized.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            ClearFields();
        }

        private void ClearFields()
        {
            FirstName = LastName = Company = Email = Phone = Address = string.Empty;
        }

        public Window CurrentWindow { get; set; }

        private void Cancel(object parameter)
        {
            ClearFields();
            CurrentWindow?.Close();
        }


    }
}

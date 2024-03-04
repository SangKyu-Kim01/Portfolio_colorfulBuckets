using System;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;

namespace PaintShopManagement.ViewModels
{
    internal class AddOrderViewModel : ViewModelBase
    {
        private readonly PaintShopDbContext _dbContext;

        // Constructor to initialize the PaintShopDbContext
        public AddOrderViewModel(PaintShopDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            CreateOrderCommand = new RelayCommand(CreateOrder, CanCreateOrder);
            CancelCommand = new RelayCommand(Cancel);

            OrderDate = DateTimeOffset.Now;
        }

        // Fields
        private int _userId;
        private int _customerId;
        private int _inventoryId;
        private int _itemQuantity;
        private DateTimeOffset _orderDate;
        private string _errorMessage;

        // Properties 
        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public int CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                OnPropertyChanged(nameof(CustomerId));
            }
        }

        public int InventoryId
        {
            get => _inventoryId;
            set
            {
                _inventoryId = value;
                OnPropertyChanged(nameof(InventoryId));
            }
        }

        public int ItemQuantity
        {
            get => _itemQuantity;
            set
            {
                _itemQuantity = value;
                OnPropertyChanged(nameof(ItemQuantity));
            }
        }

        public DateTimeOffset OrderDate
        {
            get => _orderDate;
            set
            {
                _orderDate = value;
                OnPropertyChanged(nameof(OrderDate));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // Commands
        public ICommand CreateOrderCommand { get; }
        public ICommand CancelCommand { get; }

        //Validation
        private bool CanCreateOrder(object parameter)
        {
            // Implement validation logic to check if the provided values are integers
            return UserId >= 0 &&
                   CustomerId >= 0 &&
                   InventoryId >= 0 &&
                   ItemQuantity >= 0;
        }

        private void CreateOrder(object parameter)
        {
            // Check if all required fields are filled
            if (UserId == 0 || CustomerId == 0 || InventoryId == 0 || ItemQuantity <= 0 || OrderDate == null)
            {
                ErrorMessage = "Please fill all required fields.";
                return;
            }

            try
            {
                var order = new Orders(UserId, CustomerId, InventoryId, ItemQuantity, OrderDate);

                if (_dbContext != null)
                {
                    _dbContext.Orders.Add(order);
                    _dbContext.SaveChanges();
                    // Optional
                    ErrorMessage = "Order created successfully.";
                }
                else
                {
                    ErrorMessage = "Database context is not initialized.";
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    var innerExceptionMessage = GetInnerExceptionMessage(ex.InnerException);
                    ErrorMessage = $"An error occurred while updating the database: {innerExceptionMessage}";
                }
                else
                {
                    ErrorMessage = $"An error occurred while updating the database: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            ClearFields();
        }

        private string GetInnerExceptionMessage(Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            return exception.Message;
        }



        private void ClearFields()
        {
            UserId = CustomerId = InventoryId = ItemQuantity = 0;
            OrderDate = DateTimeOffset.Now;
        }

        public Window CurrentWindow { get; set; }

        private void Cancel(object parameter)
        {
            ClearFields();
            CurrentWindow?.Close();
        }
    }
}

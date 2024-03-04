using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace PaintShopManagement.ViewModels
{
    internal class EditOrderViewModel : ViewModelBase
    {
        private readonly Orders _order;
        private readonly OrderViewModel _orderViewModel;
        private readonly Window _currentWindow;

        public EditOrderViewModel(Orders order, OrderViewModel orderViewModel, Window currentWindow)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
            _orderViewModel = orderViewModel ?? throw new ArgumentNullException(nameof(orderViewModel));
            _currentWindow = currentWindow ?? throw new ArgumentNullException(nameof(currentWindow));

            InitializeProperties();

            OrderDate = DateTimeOffset.Now;
        }

        private void InitializeProperties()
        {
            UserId = _order.userId;
            CustomerId = _order.customerId;
            InventoryId = _order.inventoryId;
            ItemQuantity = _order.itemQuantity;
            OrderDate = _order.orderDate;
        }

        // Properties 
        private int _userId;
        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        private int _customerId;
        public int CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                OnPropertyChanged(nameof(CustomerId));
            }
        }

        private int _inventoryId;
        public int InventoryId
        {
            get => _inventoryId;
            set
            {
                _inventoryId = value;
                OnPropertyChanged(nameof(InventoryId));
            }
        }

        private int _itemQuantity;
        public int ItemQuantity
        {
            get => _itemQuantity;
            set
            {
                _itemQuantity = value;
                OnPropertyChanged(nameof(ItemQuantity));
            }
        }

        private DateTimeOffset _orderDate;
        public DateTimeOffset OrderDate
        {
            get => _orderDate;
            set
            {
                _orderDate = value;
                OnPropertyChanged(nameof(OrderDate));
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

        public ICommand UpdateOrderCommand => new RelayCommand(UpdateOrder);
        public ICommand UpdateCloseCommand => new RelayCommand(UpdateClose);

        private void UpdateOrder(object parameter)
        {
            try
            {
                _order.userId = UserId;
                _order.customerId = CustomerId;
                _order.inventoryId = InventoryId;
                _order.itemQuantity = ItemQuantity;
                _order.orderDate = OrderDate;

                _orderViewModel.UpdateOrder(_order);
                _currentWindow?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating order: {ex.Message}";
            }
        }

        private void UpdateClose(object parameter)
        {
            // Update the order and close the window
            UpdateOrder(parameter);
            _currentWindow?.Close();
        }

        public Window CurrentWindow { get; set; }
    }
}
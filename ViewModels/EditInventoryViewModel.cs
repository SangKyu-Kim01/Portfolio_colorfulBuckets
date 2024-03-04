using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PaintShopManagement.ViewModels
{
    internal class EditInventoryViewModel : ViewModelBase
    {
        private readonly Inventory _inventory;
        private readonly InventoryViewModel _inventoryViewModel;
        private readonly Window _currentWindow;

        public EditInventoryViewModel(Inventory inventory, InventoryViewModel inventoryViewModel, Window currentWindow)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _inventoryViewModel = inventoryViewModel ?? throw new ArgumentNullException(nameof(inventoryViewModel));
            _currentWindow = currentWindow ?? throw new ArgumentNullException(nameof(currentWindow));

            InitializeProperties();
        }

        private void InitializeProperties()
        {
            ItemName = _inventory.itemName;
            Color = _inventory.color;
            Price = _inventory.price;
            Qty = _inventory.qty;
            Weight = _inventory.wt;
            Manufacture = _inventory.manufacture;
        }

        private string _itemName;
        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        private string _color;
        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        private int _qty;
        public int Qty
        {
            get => _qty;
            set
            {
                _qty = value;
                OnPropertyChanged(nameof(Qty));
            }
        }

        private string _weight;
        public string Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        private string _manufacture;
        public string Manufacture
        {
            get => _manufacture;
            set
            {
                _manufacture = value;
                OnPropertyChanged(nameof(Manufacture));
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

        public ICommand UpdateInventoryCommand => new RelayCommand(UpdateInventory);
        public ICommand UpdateCloseCommand => new RelayCommand(UpdateClose);

        private void UpdateInventory(object parameter)
        {
            try
            {
                _inventory.itemName = ItemName;
                _inventory.color = Color;
                _inventory.price = Price;
                _inventory.qty = Qty;
                _inventory.wt = Weight;
                _inventory.manufacture = Manufacture;

                _inventoryViewModel.UpdateInventoryItem(_inventory);
                _currentWindow?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating inventory item: {ex.Message}";
            }
        }

        private void UpdateClose(object parameter)
        {
            UpdateInventory(parameter);
            _currentWindow?.Close();
        }

        public Window CurrentWindow { get; set; }

    }
}
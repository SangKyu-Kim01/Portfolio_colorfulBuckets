using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PaintShopManagement.ViewModels
{
    internal class AddInventoryViewModel : ViewModelBase
    {
        private readonly PaintShopDbContext _dbContext;

        // Constructor to initialize the PaintShopDbContext
        public AddInventoryViewModel(PaintShopDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            CreateInventoryCommand = new RelayCommand(CreateInventory, CanCreateInventory);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Fields
        private string _itemName;
        private string _color;
        private decimal _price;
        private int _qty;
        private string _weight;
        private string _manufacture;
        private string _errorMessage;

        // Properties 
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public int Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                OnPropertyChanged(nameof(Qty));
            }
        }

        public string Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        public string Manufacture
        {
            get { return _manufacture; }
            set
            {
                _manufacture = value;
                OnPropertyChanged(nameof(Manufacture));
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // Commands
        public ICommand CreateInventoryCommand { get; }
        public ICommand CancelCommand { get; }

        // Constructor
        public AddInventoryViewModel()
        {
            CreateInventoryCommand = new RelayCommand(CreateInventory, CanCreateInventory);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Validation
        private bool CanCreateInventory(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ItemName) && Price > 0 && Qty > 0 && !string.IsNullOrWhiteSpace(Weight) && !string.IsNullOrWhiteSpace(Manufacture);
        }

        // Create Inventory Item
        private void CreateInventory(object parameter)
        {
            if (string.IsNullOrWhiteSpace(ItemName) || Price <= 0 || Qty <= 0 || string.IsNullOrWhiteSpace(Weight) || string.IsNullOrWhiteSpace(Manufacture))
            {
                ErrorMessage = "Please fill all required fields and provide valid inputs.";
                return;
            }

            try
            {
                // Here, Weight property contains the selected content from the ComboBox
                var inventoryItem = new Inventory(ItemName, Color, Price, Qty, Weight, Manufacture);

                if (_dbContext != null)
                {
                    _dbContext.Inventory.Add(inventoryItem);
                    _dbContext.SaveChanges();
                    ErrorMessage = "Inventory item created successfully.";
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

        // Clear input fields
        private void ClearFields()
        {
            ItemName = string.Empty;
            Color = string.Empty;
            Price = 0;
            Qty = 0;
            Weight = string.Empty;
            Manufacture = string.Empty;
        }

        public Window CurrentWindow { get; set; }

        private void Cancel(object parameter)
        {
            ClearFields();
            CurrentWindow?.Close();
        }

    }
}
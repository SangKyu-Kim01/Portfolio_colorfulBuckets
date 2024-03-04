using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using PaintShopManagement.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

namespace PaintShopManagement.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        private readonly PaintShopDbContext _dbContext;

        // Collections
        public ObservableCollection<Orders> Orders { get; }
        public ObservableCollection<Orders> FilteredOrders { get; private set; }

        // Commands
        public ICommand AddFormCommand { get; }
        public ICommand UpdateFormCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand ExportToPdfCommand { get; }

        // Properties
        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                FilterOrders();
            }
        }

        // Constructor
        public OrderViewModel()
        {
            _dbContext = new PaintShopDbContext();

            // Initialize collections
            Orders = new ObservableCollection<Orders>(LoadOrders());
            FilteredOrders = new ObservableCollection<Orders>(Orders);

            // Initialize commands
            AddFormCommand = new RelayCommand(AddForm);
            UpdateFormCommand = new RelayCommand(UpdateForm);
            DeleteOrderCommand = new RelayCommand(DeleteOrder);
            ExportToPdfCommand = new RelayCommand(parameter => ExportToPdf((Orders)parameter));
        }

        public void ExportToPdf(Orders selectedOrder)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    var customer = _dbContext.Customers.FirstOrDefault(c => c.customerId == selectedOrder.customerId);
                    var inventoryItem = _dbContext.Inventory.FirstOrDefault(i => i.inventoryId == selectedOrder.inventoryId);
                    var user = _dbContext.Users.FirstOrDefault(u => u.userId == selectedOrder.userId);

                    if (customer == null || inventoryItem == null || user == null)
                    {
                        MessageBox.Show("One or more details not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Document pdfDocument = new Document();
                    PdfWriter.GetInstance(pdfDocument, new FileStream(filePath, FileMode.Create));
                    pdfDocument.Open();

                    // Add receipt header
                    Paragraph header = new Paragraph("Colorful Buckets");
                    header.Alignment = Element.ALIGN_CENTER;
                    header.SpacingAfter = 10f;
                    pdfDocument.Add(header);

                    // Add order details
                    Paragraph orderDetails = new Paragraph();
                    orderDetails.Add($"Order ID: {selectedOrder.orderId}\n");
                    orderDetails.Add($"Employee ID: {selectedOrder.userId}\n\n");
                    orderDetails.Add($"Order Date: {selectedOrder.orderDate}\n");
                    pdfDocument.Add(orderDetails);

                    // Add customer information
                    Paragraph customerInfo = new Paragraph();
                    customerInfo.Add($"Customer Name: {customer.firstName} {customer.lastName}\n");
                    customerInfo.Add($"Company: {customer.company}\n");
                    customerInfo.Add($"Email: {customer.email}\n");
                    customerInfo.Add($"Phone Number: {customer.phone}\n");
                    customerInfo.Add($"Address: {customer.address}\n\n");
                    pdfDocument.Add(customerInfo);

                    // Add details of purchased item
                    Paragraph itemDetails = new Paragraph();
                    itemDetails.Add($"Item Name: {inventoryItem.itemName}\n");
                    itemDetails.Add($"Color: {inventoryItem.color}\n");
                    itemDetails.Add($"Manufacturer: {inventoryItem.manufacture}\n");
                    itemDetails.Add($"Weight: {inventoryItem.wt}\n");
                    itemDetails.Add($"Price: {inventoryItem.price}\n");
                    itemDetails.Add($"Quantity: {selectedOrder.itemQuantity}\n\n");
                    pdfDocument.Add(itemDetails);

                    // Calculate total price
                    decimal totalPrice = inventoryItem.price * selectedOrder.itemQuantity;

                    // Add total order price
                    Paragraph totalPriceParagraph = new Paragraph($"Total Price: {totalPrice}\n\n");
                    totalPriceParagraph.Alignment = Element.ALIGN_RIGHT;
                    pdfDocument.Add(totalPriceParagraph);

                    // Add footer
                    Paragraph footer = new Paragraph("Thank you for your order!");
                    footer.Alignment = Element.ALIGN_CENTER;
                    footer.SpacingBefore = 10f;
                    pdfDocument.Add(footer);

                    pdfDocument.Close();

                    MessageBox.Show($"Receipt exported to {filePath}", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show($"Error exporting receipt to PDF: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load orders from the database
        private IQueryable<Orders> LoadOrders()
        {
            try
            {
                var orders = _dbContext.Orders.ToList();
                Console.WriteLine($"Number of orders retrieved: {orders.Count}");
                return orders.AsQueryable();
            }
            catch (Exception ex)
            {
                // Error handling
                Console.WriteLine($"Error loading orders: {ex.Message}");
                return Enumerable.Empty<Orders>().AsQueryable();
            }
        }

        // Filter orders based on search term
        private void FilterOrders()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredOrders = new ObservableCollection<Orders>(Orders);
            }
            else
            {
                var searchTermLower = SearchTerm.ToLower();
                var filteredList = Orders.Where(order =>
                    order.orderId.ToString().ToLower().Contains(searchTermLower) ||
                    order.orderDate.ToString().ToLower().Contains(searchTermLower)
                ).ToList();

                FilteredOrders = new ObservableCollection<Orders>(filteredList);
            }

            OnPropertyChanged(nameof(FilteredOrders));
        }

        // Add a new order
        private void AddForm(object parameter)
        {
            var addOrderView = new AddOrderView(_dbContext);
            addOrderView.ShowDialog();

            // Reload orders after addition
            RefreshOrders();
        }

        // Update an order
        private void UpdateForm(object parameter)
        {
            if (parameter is Orders selectedOrder)
            {
                var editOrderView = new EditOrderView(selectedOrder, this);
                editOrderView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an order to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Delete an order
        private void DeleteOrder(object parameter)
        {
            if (parameter is Orders order)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the selected order?\n\nOrder ID: {order.orderId}", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Orders.Remove(order);
                        _dbContext.Orders.Remove(order);
                        _dbContext.SaveChanges();

                        // Reload orders after deletion
                        RefreshOrders();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting order: {ex.Message}");
                        MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Refresh orders
        private void RefreshOrders()
        {
            Orders.Clear();
            LoadOrders().ToList().ForEach(o => Orders.Add(o));

            // Reapply filtering
            FilterOrders();
        }

        // Edit an order
        public void EditOrder(Orders order)
        {
            var editOrderView = new EditOrderView(order, this);
            var editOrderViewModel = new EditOrderViewModel(order, this, editOrderView);

            editOrderView.DataContext = editOrderViewModel;

            editOrderView.ShowDialog();
        }


        // Update an order 
        public void UpdateOrder(Orders order)
        {
            try
            {
                _dbContext.SaveChanges();

                // Reload orders after update
                RefreshOrders();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order: {ex.Message}");
            }
        }

        // Selected order
        private Orders _selectedOrder;
        public Orders SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

    }
}

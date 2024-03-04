using iTextSharp.text.pdf;
using iTextSharp.text;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using PaintShopManagement.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data;
using System.Windows.Controls;
using Microsoft.Win32;

namespace PaintShopManagement.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        private readonly PaintShopDbContext _dbContext;

        // Collections
        public ObservableCollection<Customers> Customers { get; }
        public ObservableCollection<Customers> FilteredCustomers { get; private set; }

        // Commands
        public ICommand AddFormCommand { get; }
        public ICommand UpdateFormCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
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
                FilterCustomers();
            }
        }

        // Constructor
        public CustomerViewModel()
        {
            _dbContext = new PaintShopDbContext();

            // Initialize collections
            Customers = new ObservableCollection<Customers>(LoadCustomers());
            FilteredCustomers = new ObservableCollection<Customers>(Customers);

            // Initialize commands
            AddFormCommand = new RelayCommand(AddForm);
            UpdateFormCommand = new RelayCommand(UpdateForm);
            DeleteCustomerCommand = new RelayCommand(DeleteCustomer);
            ExportToPdfCommand = new RelayCommand(parameter => ExportToPdf((DataGrid)parameter));
        }

        // Export to PDF
        public void ExportToPdf(DataGrid dataGrid)
        {
            try
            {
                // Create a save file dialog
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";

                // Show save file dialog and wait for user action
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Get the file path chosen by the user
                    string filePath = saveFileDialog.FileName;

                    // Create a new PDF document
                    Document pdfDocument = new Document(PageSize.A4, 10f, 10f, 10f, 10f); // using
                    PdfWriter.GetInstance(pdfDocument, new FileStream(filePath, FileMode.Create));
                    pdfDocument.Open();

                    // Create a PDF table with the same number of columns as the DataGrid
                    PdfPTable pdfTable = new PdfPTable(dataGrid.Columns.Count);
                    pdfTable.DefaultCell.Padding = 3;
                    pdfTable.WidthPercentage = 100;
                    pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                    // Add DataGrid headers to the PDF table
                    foreach (DataGridColumn column in dataGrid.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()));
                        pdfTable.AddCell(cell);
                    }

                    // Add DataGrid rows to the PDF table
                    foreach (var item in dataGrid.Items)
                    {
                        foreach (DataGridColumn column in dataGrid.Columns)
                        {
                            var content = (column.GetCellContent(item) as TextBlock)?.Text;
                            content = content == null ? "" : content;
                            PdfPCell cell = new PdfPCell(new Phrase(content));
                            pdfTable.AddCell(cell);
                        }
                    }

                    // Add PDF table to the PDF document
                    pdfDocument.Add(pdfTable);
                    pdfDocument.Close();

                    MessageBox.Show($"DataGrid exported to {filePath}", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show($"Error exporting DataGrid to PDF: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load customers from the database
        private IQueryable<Customers> LoadCustomers()
        {
            try
            {
                var customers = _dbContext.Customers.ToList();
                Console.WriteLine($"Number of customers retrieved: {customers.Count}");
                return customers.AsQueryable();
            }
            catch (Exception ex)
            {
                // Error handling
                Console.WriteLine($"Error loading customers: {ex.Message}");
                return Enumerable.Empty<Customers>().AsQueryable();
            }
        }

        // Filter customers based on search term
        private void FilterCustomers()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredCustomers = new ObservableCollection<Customers>(Customers);
            }
            else
            {
                var filteredList = Customers.Where(c =>
                    c.firstName.ToLower().Contains(SearchTerm.ToLower()) ||
                    c.lastName.ToLower().Contains(SearchTerm.ToLower()) ||
                    c.company.ToLower().Contains(SearchTerm.ToLower())
                ).ToList();

                FilteredCustomers = new ObservableCollection<Customers>(filteredList);
            }

            OnPropertyChanged(nameof(FilteredCustomers));
        }

        // Add a new customer
        private void AddForm(object parameter)
        {
            var addCustomerView = new AddCustomerView(_dbContext);
            addCustomerView.ShowDialog();

            // Reload customers after addition
            RefreshCustomers();
        }

        // Update a customer
        public void UpdateCustomer(Customers customer)
        {
            try
            {
                _dbContext.SaveChanges();

                // Reload customers after update
                RefreshCustomers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
            }
        }

        // Delete a customer
        private void DeleteCustomer(object parameter)
        {
            if (parameter is Customers customer)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the selected customer?\n\nCustomer: {customer.firstName} {customer.lastName}", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Customers.Remove(customer);
                        _dbContext.Customers.Remove(customer);
                        _dbContext.SaveChanges();

                        // Reload customers after deletion
                        RefreshCustomers();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting customer: {ex.Message}");
                        MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Refresh customers
        private void RefreshCustomers()
        {
            Customers.Clear();
            LoadCustomers().ToList().ForEach(c => Customers.Add(c));

            // Reapply filtering
            FilterCustomers();
        }

        // Edit a customer
        public void EditCustomer(Customers customer)
        {
            var editCustomerView = new EditCustomerView(customer, this);
            var editCustomerViewModel = new EditCustomerViewModel(customer, this, editCustomerView);

            editCustomerView.DataContext = editCustomerViewModel;

            editCustomerView.ShowDialog();
        }

        // Open update form
        private void UpdateForm(object parameter)
        {
            if (parameter is Customers selectedCustomer)
            {
                var editCustomerView = new EditCustomerView(selectedCustomer, this);

                // Show the EditCustomerView
                editCustomerView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a customer to update.");
            }
        }

        // Selected customer
        private Customers _selectedCustomer;
        public Customers SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }
    }
}

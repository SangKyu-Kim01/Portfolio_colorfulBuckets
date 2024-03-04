using iTextSharp.text.pdf;
using iTextSharp.text;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using PaintShopManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace PaintShopManagement.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        private readonly PaintShopDbContext _dbContext;

        // Collections
        public ObservableCollection<Inventory> InventoryItems { get; }
        public ObservableCollection<Inventory> FilteredInventoryItems { get; private set; }

        // Commands
        public ICommand AddFormCommand { get; }
        public ICommand UpdateFormCommand { get; }
        public ICommand DeleteInventoryItemCommand { get; }
        public ICommand ExportToPdfCommand { get; } // Added

        // Properties
        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                FilterInventoryItems();
            }
        }

        // Constructor
        public InventoryViewModel()
        {
            _dbContext = new PaintShopDbContext();

            // Initialize collections
            InventoryItems = new ObservableCollection<Inventory>(LoadInventoryItems());
            FilteredInventoryItems = new ObservableCollection<Inventory>(InventoryItems);

            // Initialize commands
            AddFormCommand = new RelayCommand(AddForm);
            UpdateFormCommand = new RelayCommand(UpdateForm);
            DeleteInventoryItemCommand = new RelayCommand(DeleteInventory);
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


        // Load inventory items from the database
        private IQueryable<Inventory> LoadInventoryItems()
        {
            try
            {
                var items = _dbContext.Inventory.ToList();
                Console.WriteLine($"Number of inventory items retrieved: {items.Count}");
                return items.AsQueryable();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading inventory items: {ex.Message}");
                return Enumerable.Empty<Inventory>().AsQueryable();
            }
        }

        // Filter inventory items based on search term
        private void FilterInventoryItems()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredInventoryItems = new ObservableCollection<Inventory>(InventoryItems);
            }
            else
            {
                var filteredList = InventoryItems.Where(i =>
                    i.itemName.ToLower().Contains(SearchTerm.ToLower())
                // Add more conditions for filtering if needed
                ).ToList();

                FilteredInventoryItems = new ObservableCollection<Inventory>(filteredList);
            }

            OnPropertyChanged(nameof(FilteredInventoryItems));
        }

        // Add a new inventory item
        private void AddForm(object parameter)
        {
            var addInventoryView = new AddInventoryView(_dbContext);
            addInventoryView.ShowDialog();

            // Reload inventory items after addition
            RefreshInventoryItems();
        }

        // Update an inventory item
        public void UpdateInventoryItem(Inventory item)
        {
            try
            {
                _dbContext.SaveChanges();

                // Reload inventory items after update
                RefreshInventoryItems();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating inventory item: {ex.Message}");
            }
        }

        // Delete an inventory item
        private void DeleteInventory(object parameter)
        {
            if (parameter is Inventory item)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the selected inventory item?\n\nItem Name: {item.itemName}", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        InventoryItems.Remove(item);
                        _dbContext.Inventory.Remove(item);
                        _dbContext.SaveChanges();

                        // Reload inventory items after deletion
                        RefreshInventoryItems();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting inventory item: {ex.Message}");
                        MessageBox.Show($"Error deleting inventory item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an inventory item to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Refresh inventory items
        private void RefreshInventoryItems()
        {
            InventoryItems.Clear();
            LoadInventoryItems().ToList().ForEach(i => InventoryItems.Add(i));

            // Reapply filtering
            FilterInventoryItems();
        }

        // Edit an inventory item
        public void EditInventoryItem(Inventory item)
        {
            var editInventoryView = new EditInventoryView(item, this);
            var editInventoryViewModel = new EditInventoryViewModel(item, this, editInventoryView);

            editInventoryView.DataContext = editInventoryViewModel;

            editInventoryView.ShowDialog();
        }

        // Open update form
        private void UpdateForm(object parameter)
        {
            if (parameter is Inventory selectedItem)
            {
                var editInventoryView = new EditInventoryView(selectedItem, this);
                editInventoryView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an inventory item to update.");
            }
        }

        // Selected inventory item
        private Inventory _selectedInventoryItem;
        public Inventory SelectedInventoryItem
        {
            get { return _selectedInventoryItem; }
            set
            {
                _selectedInventoryItem = value;
                OnPropertyChanged(nameof(SelectedInventoryItem));
            }
        }
    }
}

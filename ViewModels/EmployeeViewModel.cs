using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using iTextSharp.text.pdf;
using iTextSharp.text;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;
using PaintShopManagement.Views;
using Microsoft.Win32;

namespace PaintShopManagement.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {

        private readonly PaintShopDbContext _dbContext;

        private ObservableCollection<Users> _employees = new ObservableCollection<Users>();
        private bool _isDataLoaded;

        public ICommand UserCreateFormCommand { get; }
        public ICommand UserUpdateFormCommand { get; }
        public ICommand UserDeleteCommand { get; }
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
                FilterUsers();
            }
        }

        private Users _selectedUser;

        public Users SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public EmployeeViewModel()
        {
            _dbContext = new PaintShopDbContext();

            UserCreateFormCommand = new RelayCommand(AddForm);
            UserUpdateFormCommand = new RelayCommand(UpdateForm, CanUserUpdate);
            UserDeleteCommand = new RelayCommand(DeleteUser);
            ExportToPdfCommand = new RelayCommand(parameter => ExportToPdf((DataGrid)parameter));

            // Initialize collections
            Users = new ObservableCollection<Users>(LoadUsers());
        }


        //----------------    Search Filter Users   ---------------------------------
        // Collections
        public ObservableCollection<Users> Users { get; }
        public ObservableCollection<Users> FilteredUsers { get; private set; }

       
        // Filter users based on search term
        private void FilterUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                Employees = new ObservableCollection<Users>(Users);  
            }
            else
            {
                var filteredList = Users.Where(u =>
                    u.firstName.ToLower().Contains(SearchTerm.ToLower()) ||
                    u.lastName.ToLower().Contains(SearchTerm.ToLower()) ||
                    u.userName.ToLower().Contains(SearchTerm.ToLower())

                ).ToList();

                Employees = new ObservableCollection<Users>(filteredList);
            }

            OnPropertyChanged(nameof(Employees));
        }

        // Load users from the database
        private IQueryable<Users> LoadUsers()
        {
            try
            {
                var users = _dbContext.Users.ToList();
                Console.WriteLine($"Number of users retrieved: {users.Count}");
                return users.AsQueryable();
            }
            catch (Exception ex)
            {
                // Error handling
                Console.WriteLine($"Error loading users: {ex.Message}");
                return Enumerable.Empty<Users>().AsQueryable();
            }
        }

        //-----------------------------------------------------------------------


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

        //-----------------------------------------------------------------------

        private void DeleteUser(object obj)
        {
            using (var dbContext = new PaintShopDbContext())
            {
                if (SelectedUser == null)
                {
                    return;
                }

                var entry = dbContext.Entry(SelectedUser);
                if (entry.State == EntityState.Detached)
                {
                    dbContext.Users.Attach(SelectedUser);
                }

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {SelectedUser.userName}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    dbContext.Users.Remove(SelectedUser);
                    dbContext.SaveChanges();

                    refresh();

                    // Clear the selected user after deletion
                    SelectedUser = null;
                }
            }
        }



        private bool CanUserUpdate(object parameter)
        {
            Console.WriteLine($" Is SelectedUser ? : {SelectedUser}");
            return SelectedUser != null;
        }

        private void UpdateForm(object obj)
        {
            var updateUserView = new UpdateUserView(_dbContext, SelectedUser, this);
            updateUserView.ShowDialog();
        }

        private void AddForm(object obj)
        {
            var addUserView = new AddUserView(_dbContext, this);
            addUserView.ShowDialog();
        }


        public ObservableCollection<Users> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        public async Task InitializeAsync()
        {
            if (!_isDataLoaded)
            {
                try
                {
                    using (var dbContext = new PaintShopDbContext())
                    {
                        var employeeData = await dbContext.Users.ToListAsync();

                        Employees = new ObservableCollection<Users>(employeeData);
                    }

                    _isDataLoaded = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading data: {ex.Message}");
                    // TODO: error Message handling for better user experience
                }
            }

        }

        public async Task refresh()
        {
            try
            {
                using (var dbContext = new PaintShopDbContext())
                {
                    var employeeData = await dbContext.Users.ToListAsync();

                    Employees = new ObservableCollection<Users>(employeeData);
                }

                _isDataLoaded = true;
                Console.WriteLine("After InitializeAsync: _isDataLoaded = " + _isDataLoaded);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

    }

}

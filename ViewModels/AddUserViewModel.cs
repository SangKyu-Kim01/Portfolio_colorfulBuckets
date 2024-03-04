using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;

namespace PaintShopManagement.ViewModels
{
    internal class AddUserViewModel : ViewModelBase
    {

        private EmployeeViewModel _employeeViewModel;
        public string UserName { get; set; }
        public string PlainTextPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        private int _selectedPosition;
        public int SelectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                _selectedPosition = value;
                OnPropertyChanged(nameof(SelectedPosition));
            }
        }
        public byte[] ImageBytes { get; set; }

        private string _tbxFilePath;
        public string TbxFilePath
        {
            get { return _tbxFilePath; }
            set
            {
                _tbxFilePath = value;
                OnPropertyChanged(nameof(TbxFilePath));
            }
        }


        public ICommand CreateUserCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand BrowseButtonCommand { get; set; }
        public string ErrorMessage { get; set; }


        private readonly PaintShopDbContext _dbContext;
        public AddUserViewModel(PaintShopDbContext dbContext, EmployeeViewModel employeeViewModel)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _employeeViewModel = employeeViewModel ?? throw new ArgumentNullException(nameof(employeeViewModel));
            InitializeCommands();
        }

        public AddUserViewModel()
        {
            // if no context is provided, create a new one
            _dbContext = new PaintShopDbContext();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            CreateUserCommand = new RelayCommand(ExecuteCreateUserCommand);
            BrowseButtonCommand = new RelayCommand(ExecuteBrowseButtonCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public Window CurrentWindow { get; set; }

        private void ExecuteCancelCommand(object obj)
        {
            CurrentWindow?.Close();
        }

        private void ExecuteCreateUserCommand(object obj)
        {
            try
            {
                // Validate input fields
                if (string.IsNullOrEmpty(UserName) ||
                    string.IsNullOrEmpty(PlainTextPassword) ||
                    string.IsNullOrEmpty(FirstName) ||
                    string.IsNullOrEmpty(LastName) ||
                    string.IsNullOrEmpty(Email) ||
                    string.IsNullOrEmpty(Phone))
                {
                    string errorMessage = "Please fill in the following required fields:\n";

                    if (string.IsNullOrEmpty(UserName))
                        errorMessage += "- UserName\n";

                    if (string.IsNullOrEmpty(PlainTextPassword))
                        errorMessage += "- Password\n";

                    if (string.IsNullOrEmpty(FirstName))
                        errorMessage += "- FirstName\n";

                    if (string.IsNullOrEmpty(LastName))
                        errorMessage += "- LastName\n";

                    if (string.IsNullOrEmpty(Email))
                        errorMessage += "- Email\n";

                    if (string.IsNullOrEmpty(Phone))
                        errorMessage += "- Phone\n";

                    MessageBox.Show(errorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;  // Stop execution if any field is empty
                }

                string hashedPassword = HashPassword(PlainTextPassword);

                // Get the data from your UI controls
                string userName = UserName;
                string password = hashedPassword;
                string firstName = FirstName;
                string lastName = LastName;
                string email = Email;
                string phone = Phone;
                int selectedPositionItem = SelectedPosition;
                byte[] profilePic = ImageBytes;

                AddUserToDatabase(userName, password, firstName, lastName, email, phone, selectedPositionItem, profilePic);

                CurrentWindow?.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void warningInputNull()
        {
            MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;  // Stop execution if any field is empty
        }

        private void ExecuteBrowseButtonCommand(object obj)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select an image file",
                    Filter = "Image files|*.jpg;*.jpeg;*.png;*.png;*.gif|All files|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    // Read the selected image file and convert it to byte[]
                    byte[] imageBytes = ConvertImageToByteArray(openFileDialog.FileName);

                    // Set the byte[] in the ViewModel
                    ImageBytes = imageBytes;
                    TbxFilePath = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private byte[] ConvertImageToByteArray(string imagePath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath));

                BitmapEncoder encoder = null;

                // Determine the image format based on the file extension
                string fileExtension = System.IO.Path.GetExtension(imagePath).ToLower();

                switch (fileExtension)
                {
                    case ".jpg":
                    case ".jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;

                    case ".gif":
                        encoder = new GifBitmapEncoder();
                        break;

                    default:
                        throw new NotSupportedException("Unsupported image format.");
                }

                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error converting image to byte array: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private string HashPassword(string plainTextPassword)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the generated salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(plainTextPassword, salt, 10000))
            {
                byte[] hashedPassword = pbkdf2.GetBytes(32); // 32 bytes for a 256-bit key
                byte[] saltedHash = salt.Concat(hashedPassword).ToArray();
                return Convert.ToBase64String(saltedHash);
            }
        }

        // Refresh 
        private void RefreshUsers()
        {
            _employeeViewModel.refresh();
        }

        // Method to add user to the database
        public void AddUserToDatabase(string _userName, string _password, string _firstName, string _lastName, string _email,
            string _phone, int _selectedPositionItem, byte[] _profilePic)
        {
            try
            {
                Users newUser = new Users
                {
                    userName = _userName,
                    password = _password,
                    firstName = _firstName,
                    lastName = _lastName,
                    email = _email,
                    phone = _phone,
                    position = _selectedPositionItem,
                    profilePic = _profilePic
                };
                // Add the user to the database
                _dbContext.Users.Add(newUser);

                try
                {
                    // Save changes to the database
                    _dbContext.SaveChanges();

                    // Reload customers after update
                    RefreshUsers();

                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Console.WriteLine($"Entity: {validationErrors.Entry.Entity.GetType().Name}, Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                        }
                    }
                }


                MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Close the current window if available
                CurrentWindow?.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

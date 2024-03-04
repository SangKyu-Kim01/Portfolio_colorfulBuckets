using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PaintShopManagement.Commands;
using PaintShopManagement.Models;

namespace PaintShopManagement.ViewModels
{
    internal class UpdateUserViewModel : ViewModelBase
    {

        private readonly PaintShopDbContext _dbContext;
        private Users _selectedUser;
        private int _selectedPosition;
        private EmployeeViewModel _employeeViewModel;

        public string UserName { get; set; }
        public string PlainTextPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
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

        public ICommand UpdateUserCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand BrowseButtonCommand { get; set; }
        public string ErrorMessage { get; set; }


        public UpdateUserViewModel(PaintShopDbContext dbContext, Users selectedUser, EmployeeViewModel employeeViewModel)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _selectedUser = selectedUser ?? throw new ArgumentNullException(nameof(selectedUser));
            _employeeViewModel = employeeViewModel ?? throw new ArgumentNullException(nameof(employeeViewModel));
            InitializeCommands();
            InitializeUserInfo(_selectedUser);
        }

        private void InitializeUserInfo(Users _selectedUser)
        {
            // Set initial property values based on the selectedUser
            UserName = _selectedUser.userName;
            //PlainTextPassword = _selectedUser.password;
            PlainTextPassword = "";
            FirstName = _selectedUser.firstName;
            LastName = _selectedUser.lastName;
            Email = _selectedUser.email;
            Phone = _selectedUser.phone;
            // Map the Tag value to SelectedPosition
            MapPositionTagToSelectedPosition(_selectedUser.position);
        }

        private void MapPositionTagToSelectedPosition(int positionTag)
        {
            // Tag 1 : Admin,  Tag 2 : Employee
            SelectedPosition = positionTag;
        }

        public UpdateUserViewModel(Users selectedUser)
        {
            _dbContext = new PaintShopDbContext();
            _selectedUser = selectedUser ?? throw new ArgumentNullException(nameof(selectedUser));
            InitializeCommands();
            InitializeUserInfo(_selectedUser);
        }

        private void InitializeCommands()
        {
            UpdateUserCommand = new RelayCommand(ExecuteUpdateUserCommand);
            BrowseButtonCommand = new RelayCommand(ExecuteBrowseButtonCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public Window CurrentWindow { get; set; }


        private void ExecuteUpdateUserCommand(object obj)
        {
            try
            {
                using (var _dbContext = new PaintShopDbContext())
                {
                    // Attach the selected user to the context if not already attached
                    _dbContext.Users.Attach(_selectedUser);

                    // Update the selected user's properties
                    _selectedUser.userName = UserName;

                    if (!string.IsNullOrEmpty(PlainTextPassword))
                    {
                        _selectedUser.password = HashPassword(PlainTextPassword);
                    }
                    _selectedUser.firstName = FirstName;
                    _selectedUser.lastName = LastName;
                    _selectedUser.email = Email;
                    _selectedUser.phone = Phone;
                    _selectedUser.position = SelectedPosition; // Map SelectedPosition back to position tag

                    // Update the image if it's selected
                    if (ImageBytes != null)
                    {
                        _selectedUser.profilePic = ImageBytes;
                    }

                    // Save changes to the database
                    _dbContext.SaveChanges();

                    // Reload customers after update
                    RefreshUsers();

                    MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Close the window
                    CurrentWindow?.Close();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Refresh 
        private void RefreshUsers()
        {
            _employeeViewModel.refresh();
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

        private void ExecuteCancelCommand(object obj)
        {
            CurrentWindow?.Close();
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

    }
}

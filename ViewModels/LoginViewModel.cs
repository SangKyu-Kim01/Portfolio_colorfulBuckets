using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PaintShopManagement.Models;
using PaintShopManagement.Repositories;

namespace PaintShopManagement.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        // Field
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;

        private IUserRepository userRepository;

        // Properties 
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }

        // Constructor
        public LoginViewModel()
        {
            //Add the UserRepository
            userRepository = new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPwdCommand("", ""));
        }

        private void ExecuteRecoverPwdCommand(string username, string id)
        {
            throw new NotImplementedException();
        }

        private void ExecuteLoginCommand(object obj)
        {
            // Establish the data binding between the properties of the view and the viewmodel
            var isValidUser = userRepository.AuthenticateUser(new NetworkCredential(Username, Password));

            //Validation
            if (isValidUser)
            {
                // Retrieve user information, including position
                var user = userRepository.GetByUsername(Username);

                // This property allows to establish the id of the user that is executing the current thread
                Thread.CurrentPrincipal = new GenericPrincipal(
                    new GenericIdentity($"{Username}|{user.Position}"), null);
                IsViewVisible = false;
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            // Login Validation
            bool validData;
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 || Password == null || Password.Length < 3) validData = false;
            else validData = true;
            return validData;
        }
    }
}

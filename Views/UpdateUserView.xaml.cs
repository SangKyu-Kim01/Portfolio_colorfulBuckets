using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using PaintShopManagement.Models;
using PaintShopManagement.ViewModels;

namespace PaintShopManagement.Views
{
    /// <summary>
    /// Interaction logic for UpdateUserView.xaml
    /// </summary>
    public partial class UpdateUserView : Window
    {
        private UpdateUserViewModel viewModel;
        public UpdateUserView(PaintShopDbContext dbContext, Users selectedUser, EmployeeViewModel employeeViewModel)
        {
            InitializeComponent();
            viewModel = new UpdateUserViewModel(dbContext, selectedUser, employeeViewModel);
            viewModel.CurrentWindow = this;
            DataContext = viewModel;
        }
       
    }
}

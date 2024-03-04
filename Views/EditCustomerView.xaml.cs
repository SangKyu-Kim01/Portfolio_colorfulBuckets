using PaintShopManagement.Models;
using PaintShopManagement.ViewModels;
using System;
using System.Collections.Generic;
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

namespace PaintShopManagement.Views
{
    /// <summary>
    /// Interaction logic for EditCustomerView.xaml
    /// </summary>
    public partial class EditCustomerView : Window
    {
        public EditCustomerView(Customers customer, CustomerViewModel parentViewModel)
        {
            InitializeComponent();

            var currentWindow = this;

            var viewModel = new EditCustomerViewModel(customer, parentViewModel, currentWindow);

            DataContext = viewModel;
        }

    }
}

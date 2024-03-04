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
    /// Interaction logic for AddInventoryView.xaml
    /// </summary>
    public partial class AddInventoryView : Window
    {
        public AddInventoryView(PaintShopDbContext dbContext)
        {
            InitializeComponent();

            var viewModel = new AddInventoryViewModel(dbContext);
            viewModel.CurrentWindow = this;
            DataContext = viewModel;
        }

    }
}

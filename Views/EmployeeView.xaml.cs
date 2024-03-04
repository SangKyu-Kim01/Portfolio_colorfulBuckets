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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PaintShopManagement.ViewModels;

namespace PaintShopManagement.Views
{
    /// <summary>
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class EmployeeView : UserControl
    {
        private readonly EmployeeViewModel _viewModel;
        public EmployeeView()
        {
            InitializeComponent();
            _viewModel = new EmployeeViewModel();
            DataContext = _viewModel;

            Loaded += async (_, __) => await _viewModel.InitializeAsync();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string propertyName = e.PropertyName;
            double maxWidth = 30;

            // adjust column width based on maxWidth
            DataGridTextColumn column = e.Column as DataGridTextColumn;
            if (column != null)
            {
                column.Width = new DataGridLength(maxWidth);
            }
        }
    }
}

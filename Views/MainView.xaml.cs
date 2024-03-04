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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using PaintShopManagement.ViewModels;

namespace PaintShopManagement.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            // Full Screen
            // this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        // Resizing and moving windows using Win32 API functions
        [DllImport("user32.dll")] // To access functionality provided by windows
        /* 
         * Send message specified to the windows
         * hWnd handle to the window which msg to be sent
         * wMsg specifies the message to send
         * wParam and lParam message specific parameters
        */
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void ControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // DragMove(); 

            // New object which provides access to the underlying Win32 window handle
            WindowInteropHelper helper = new WindowInteropHelper(this);
            // Send a WM_SYSCOMMAND message
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void ControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set maximum height to the primary screen
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        // Close window
        private void btnCLose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        // Maximize window
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }
        // Minimize window
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            
        }
    }
}

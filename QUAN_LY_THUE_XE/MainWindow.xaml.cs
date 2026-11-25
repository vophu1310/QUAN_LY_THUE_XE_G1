using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QUAN_LY_THUE_XE.Views;

namespace QUAN_LY_THUE_XE
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChonMenu(btnVehicle);
            MainContentArea.Content = new VehicleView();
        }

        private void ChonMenu(Button activeButton)
        {
            var inactiveFg = (SolidColorBrush)new BrushConverter().ConvertFromString("#A2B7AD")!;
            var activeBg = (SolidColorBrush)new BrushConverter().ConvertFromString("#24473B")!;

            foreach (var btn in new[] { btnDashboard, btnBooking, btnVehicle, btnCustomer, btnRental, btnReports, btnFinance })
            {
                btn.Background = Brushes.Transparent;
                btn.Foreground = inactiveFg;
                btn.FontWeight = FontWeights.Normal;
            }

            activeButton.Background = activeBg;
            activeButton.Foreground = Brushes.White;
            activeButton.FontWeight = FontWeights.SemiBold;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ChonMenu(sender is Button b ? b : btnDashboard);
            MainContentArea.Content = new DashboardView();
        }

        private void BtnVehicle_Click(object sender, RoutedEventArgs e)
        {
            ChonMenu(btnVehicle);
            MainContentArea.Content = new VehicleView();
        }

        private void BtnCustomer_Click(object sender, RoutedEventArgs e)
        {
            ChonMenu(btnCustomer);
            MainContentArea.Content = new CustomerView();
        }

        private void BtnRental_Click(object sender, RoutedEventArgs e)
        {
            ChonMenu(sender is Button b ? b : btnRental);
            MainContentArea.Content = new RentalView();
        }

        private void BtnFinance_Click(object sender, RoutedEventArgs e)
        {
            ChonMenu(btnFinance);
            MainContentArea.Content = new FinanceView();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}
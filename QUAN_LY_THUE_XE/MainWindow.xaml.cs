using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QUAN_LY_THUE_XE.Views;

namespace QUAN_LY_THUE_XE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContentArea.Content = new DashboardView();
            SetActiveNavigation(btnDashboard);
        }

        private void SetActiveNavigation(Button activeButton)
        {
            Button?[] navigationButtons =
            [
                btnDashboard,
                FindName("btnBooking") as Button,
                btnVehicle,
                btnCustomer,
                btnRental,
                btnHandover,
                FindName("btnReports") as Button,
                btnFinance
            ];

            foreach (var button in navigationButtons)
            {
                if (button is null)
                {
                    continue;
                }

                button.Background = System.Windows.Media.Brushes.Transparent;
                button.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(169, 190, 180));
            }

            activeButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 84, 71));
            activeButton.Foreground = System.Windows.Media.Brushes.White;
        }

        // Nạp màn hình khách hàng vào vùng nội dung; CustomerView sẽ tự co giãn theo vùng này.
        private void BtnCustomer_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new CustomerView();
            SetActiveNavigation(btnCustomer);
        }

        // Nạp màn hình tổng quan với 3 chỉ số trạng thái đội xe.
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new DashboardView();
            SetActiveNavigation(sender is Button clickedButton && ReferenceEquals(clickedButton, FindName("btnReports"))
                ? clickedButton
                : btnDashboard);
        }

        // Chức năng đang phát triển — sẽ hoàn thiện ở sprint tiếp theo.
        private void BtnVehicle_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new VehicleView();
            SetActiveNavigation(btnVehicle);
        }

        // Chức năng đang phát triển — sẽ hoàn thiện ở sprint tiếp theo.
        private void BtnRental_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new RentalView();
            SetActiveNavigation(sender is Button clickedButton && ReferenceEquals(clickedButton, FindName("btnBooking"))
                ? clickedButton
                : btnRental);
        }

        private void BtnHandover_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new HandoverView();
            SetActiveNavigation(btnHandover);
        }

        private void BtnFinance_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new FinanceView();
            SetActiveNavigation(btnFinance);
        }
    }
}

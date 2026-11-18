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
        }

        // Nạp màn hình khách hàng vào vùng nội dung; CustomerView sẽ tự co giãn theo vùng này.
        private void BtnCustomer_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new CustomerView();
        }

        // Nạp màn hình tổng quan với 3 chỉ số trạng thái đội xe.
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new DashboardView();
        }

        // Chức năng đang phát triển — sẽ hoàn thiện ở sprint tiếp theo.
        private void BtnVehicle_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new VehicleView();
        }

        // Chức năng đang phát triển — sẽ hoàn thiện ở sprint tiếp theo.
        private void BtnRental_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new RentalView();
        }
    }
}

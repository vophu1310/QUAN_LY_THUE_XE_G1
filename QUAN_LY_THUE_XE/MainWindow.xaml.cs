using System.Windows;
using QUAN_LY_THUE_XE.Views;

namespace QUAN_LY_THUE_XE
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Mở sẵn VehicleView để bạn test trực tiếp
            MainContentArea.Content = new VehicleView();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new DashboardView();
        }

        private void BtnVehicle_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new VehicleView();
        }

        private void BtnRental_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new RentalView();
        }

        private void BtnCustomer_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new CustomerView();
        }
    }
}

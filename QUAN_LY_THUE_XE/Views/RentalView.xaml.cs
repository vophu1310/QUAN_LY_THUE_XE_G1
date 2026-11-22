using System.Windows;
using System.Windows.Controls;
using QUAN_LY_THUE_XE.ViewModels;

namespace QUAN_LY_THUE_XE.Views
{
    public partial class RentalView : UserControl
    {
        public RentalView()
        {
            InitializeComponent();
            // Gán ViewModel làm bộ não điều khiển cho giao diện
            DataContext = new RentalViewModel();
        }

        private void BtnLuuNhap_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Đã lưu tạm thông tin hợp đồng vào danh sách Bản nháp!",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}

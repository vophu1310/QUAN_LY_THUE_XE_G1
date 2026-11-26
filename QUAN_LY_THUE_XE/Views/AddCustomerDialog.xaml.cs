using QUAN_LY_THUE_XE.Models;
using System.Windows;

namespace QUAN_LY_THUE_XE.Views
{
    /// <summary>
    /// Popup thêm khách hàng mới.
    /// Nhận đối tượng KhachHang (đã có MaKH kế tiếp) từ ViewModel,
    /// cho người dùng nhập thông tin, rồi trả về khi bấm Lưu.
    /// </summary>
    public partial class AddCustomerDialog : Window
    {
        // Đối tượng khách hàng sẽ được trả về cho ViewModel sau khi lưu.
        public KhachHang KhachHangMoi { get; private set; }

        public AddCustomerDialog(KhachHang khachHangMoi)
        {
            InitializeComponent();
            KhachHangMoi = khachHangMoi;

            // Hiển thị mã KH dạng KH001, KH002...
            tbMaKH.Text = $"KH{khachHangMoi.MaKH:D3}";
        }

        // Kiểm tra dữ liệu form; nếu hợp lệ, gán vào model và trả kết quả cho ViewModel để lưu.
        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            // Validation: các trường bắt buộc.
            if (string.IsNullOrWhiteSpace(tbHoTen.Text)
                || string.IsNullOrWhiteSpace(tbSODT.Text)
                || string.IsNullOrWhiteSpace(tbCCCD.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ Họ tên, SĐT và CCCD.",
                    "Thiếu thông tin bắt buộc",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validation: CCCD phải đủ 12 số.
            string cccd = tbCCCD.Text.Trim();
            if (cccd.Length != 12 || !cccd.All(char.IsDigit))
            {
                MessageBox.Show(
                    "CCCD phải gồm đúng 12 chữ số.",
                    "CCCD không hợp lệ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validation: SĐT phải hợp lệ (10 chữ số, bắt đầu bằng 0).
            string sdt = tbSODT.Text.Trim();
            if (sdt.Length != 10 || !sdt.All(char.IsDigit) || sdt[0] != '0')
            {
                MessageBox.Show(
                    "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0.",
                    "SĐT không hợp lệ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Gán dữ liệu từ form vào đối tượng KhachHang.
            KhachHangMoi.HoTen = tbHoTen.Text.Trim();
            KhachHangMoi.SODT = sdt;
            KhachHangMoi.CCCD = cccd;
            KhachHangMoi.GPLX = tbGPLX.Text.Trim();

            // Đóng dialog với kết quả OK → ViewModel sẽ lưu vào SQL.
            DialogResult = true;
            Close();
        }

        // Đóng dialog với kết quả hủy để ViewModel không ghi dữ liệu.
        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}


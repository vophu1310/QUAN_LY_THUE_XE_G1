using QUAN_LY_THUE_XE.Models;
using System.Linq;
using System.Windows;

namespace QUAN_LY_THUE_XE.Views
{
    /// <summary>
    /// Popup Thêm mới & Chỉnh sửa thông tin khách hàng.
    /// Nhận đối tượng KhachHang từ ViewModel,
    /// cho người dùng nhập/sửa thông tin, rồi trả về khi bấm Lưu.
    /// </summary>
    public partial class AddCustomerDialog : Window
    {
        // Đối tượng khách hàng sẽ được trả về cho ViewModel sau khi lưu.
        public KhachHang KhachHangMoi { get; private set; }

        // [Ghi chú cho bạn Hoàng]: Constructor hỗ trợ cả 2 chế độ:
        // - isEditMode = false (mặc định): Thêm mới khách hàng
        // - isEditMode = true: Mở form chỉnh sửa thông tin khách hàng đã chọn
        public AddCustomerDialog(KhachHang khachHangMoi, bool isEditMode = false)
        {
            InitializeComponent();
            KhachHangMoi = khachHangMoi;

            // Hiển thị mã KH dạng KH001, KH002...
            tbMaKH.Text = $"KH{khachHangMoi.MaKH:D3}";

            if (isEditMode)
            {
                Title = "Chỉnh sửa khách hàng";
                txtTieuDe.Text = "Chỉnh sửa khách hàng";
                txtMoTa.Text = "Cập nhật thông tin khách hàng đã chọn";
                btnLuu.Content = "✓ Lưu thay đổi";

                // Điền sẵn thông tin khách hàng hiện tại vào form
                tbHoTen.Text = khachHangMoi.HoTen;
                tbSODT.Text = khachHangMoi.SODT;
                tbCCCD.Text = khachHangMoi.CCCD;
                tbGPLX.Text = khachHangMoi.GPLX;
            }
            else
            {
                Title = "Thêm khách hàng mới";
                txtTieuDe.Text = "Thêm khách hàng mới";
                txtMoTa.Text = "Nhập thông tin khách hàng để lưu vào hệ thống";
                btnLuu.Content = "+ Thêm khách hàng";
            }
        }

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

            // Đóng dialog với kết quả OK → ViewModel sẽ lưu vào SQL Server.
            DialogResult = true;
            Close();
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QUAN_LY_THUE_XE.Data;
using QUAN_LY_THUE_XE.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace QUAN_LY_THUE_XE.Views
{
    public partial class CustomerView : UserControl
    {
        private readonly ObservableCollection<DonThue> _rentalHistory = [];
        private List<KhachHang> _customers = [];
        private KhachHang? _selectedCustomer;
        private bool _isNewCustomer;

        public CustomerView()
        {
            // Khởi tạo các control được khai báo trong CustomerView.xaml.
            InitializeComponent();
            // Gắn danh sách lịch sử thuê vào DataGrid tương ứng.
            RentalHistoryGrid.ItemsSource = _rentalHistory;
        }

        // Sự kiện Loaded gọi hàm này để lấy dữ liệu mới nhất từ SQL Server khi mở màn hình.
        public void HienThiManHinh()
        {
            // Bắt đầu tải khách; không truyền ID nghĩa là không cần khôi phục lựa chọn cũ.
            LoadCustomers();
        }

        private void CustomerView_Loaded(object sender, RoutedEventArgs e)
        {
            // Nạp dữ liệu ngay sau khi giao diện được đưa lên màn hình.
            HienThiManHinh();
        }

        // Đọc dữ liệu ADO.NET, ánh xạ từng hàng của DataTable thành KhachHang rồi cập nhật DataGrid.
        private void LoadCustomers(int? selectedCustomerId = null)
        {
            try
            {
                // Nếu ô tìm kiếm trống thì lấy toàn bộ; nếu có nội dung thì tìm theo CCCD/SODT.
                var customerData = string.IsNullOrWhiteSpace(SearchBox.Text)
                    ? new KhachHang().LayTatCaKhachHang()
                    : new KhachHang().TimKiemKhachHang(SearchBox.Text);

                // Chuyển từng DataRow SQL thành một đối tượng KhachHang mà giao diện có thể bind.
                _customers = customerData.AsEnumerable()
                    .Select(row => new KhachHang
                    {
                        // Ép mã khách hàng từ kết quả SQL về kiểu số nguyên của model.
                        MaKH = Convert.ToInt32(row["MaKH"]),
                        // Sao chép dữ liệu CCCD, họ tên, SĐT và GPLX; cột thiếu/null được xử lý trong ReadColumn.
                        CCCD = ReadColumn(row, "CCCD"),
                        HoTen = ReadColumn(row, "HoTen"),
                        // Lấy số điện thoại từ đúng cột SODT trong DSKHACHHANG để hiển thị và điền biểu mẫu.
                        SODT = ReadColumn(row, "SODT"),
                        GPLX = ReadColumn(row, "GPLX"),
                        // Đọc cột danh sách đen nếu bảng có cột này; nếu không thì mặc định false.
                        IsBlacklisted = customerData.Columns.Contains("IsBlacklisted")
                            && !row.IsNull("IsBlacklisted")
                            && Convert.ToBoolean(row["IsBlacklisted"])
                    })
                    .ToList();

                // Cập nhật DataGrid và khôi phục khách được chọn (nếu có ID truyền vào).
                RefreshCustomerGrid(selectedCustomerId);
            }
            catch (SqlException exception)
            {
                StatusText.Text = $"Không thể tải danh sách khách hàng: {exception.Message}";
            }
            catch (InvalidOperationException exception)
            {
                StatusText.Text = $"Không thể tải danh sách khách hàng: {exception.Message}";
            }
            catch (FormatException exception)
            {
                StatusText.Text = $"Dữ liệu mã khách hàng không hợp lệ: {exception.Message}";
            }
        }

        private static string ReadColumn(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && !row.IsNull(columnName)
                ? Convert.ToString(row[columnName]) ?? string.Empty
                : string.Empty;
        }

        private void RefreshCustomerGrid(int? selectedCustomerId = null)
        {
            SearchPlaceholder.Visibility = string.IsNullOrWhiteSpace(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
            var blacklistOnly = BlacklistOnlyCheckBox.IsChecked == true;
            var visibleCustomers = _customers
                .Where(customer => !blacklistOnly || customer.IsBlacklisted)
                .ToList();

            CustomerGrid.ItemsSource = visibleCustomers;
            if (selectedCustomerId.HasValue)
            {
                CustomerGrid.SelectedItem = visibleCustomers.FirstOrDefault(customer => customer.MaKH == selectedCustomerId.Value);
            }
        }

        private void LoadRentalHistory(int customerId)
        {
            try
            {
                using var context = new AppDbContext();
                var rentals = context.DonThues
                    .AsNoTracking()
                    .Include(rental => rental.Xe)
                    .Where(rental => rental.MaKH == customerId)
                    .OrderByDescending(rental => rental.NgayBatDauThue)
                    .ToList();

                _rentalHistory.Clear();
                foreach (var rental in rentals)
                {
                    _rentalHistory.Add(rental);
                }

                HistoryHeading.Text = $"Lịch sử thuê của {_selectedCustomer?.HoTen}";
            }
            catch (SqlException exception)
            {
                StatusText.Text = $"Không thể tải lịch sử thuê: {exception.Message}";
            }
            catch (InvalidOperationException exception)
            {
                StatusText.Text = $"Không thể tải lịch sử thuê: {exception.Message}";
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCustomers(_selectedCustomer?.MaKH);
        }

        private void BlacklistOnlyCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            RefreshCustomerGrid(_selectedCustomer?.MaKH);
        }

        private void CustomerGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerGrid.SelectedItem is not KhachHang customer)
            {
                return;
            }

            _selectedCustomer = customer;
            _isNewCustomer = false;
            tbMaKhachHang.Text = $"KH{customer.MaKH:D3}";
            NameBox.Text = customer.HoTen;
            PhoneBox.Text = customer.SODT;
            IdentityBox.Text = customer.CCCD;
            LicenseBox.Text = customer.GPLX;
            BlacklistButton.Content = customer.IsBlacklisted
                ? "Gỡ khỏi danh sách đen"
                : "Đưa vào danh sách đen";
            LoadRentalHistory(customer.MaKH);
            StatusText.Text = $"Đang xem hồ sơ khách hàng #{customer.MaKH}.";
        }

        private void NewCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var customerId = new KhachHang().LayMaKHKeTiep();
                _selectedCustomer = new KhachHang
                {
                    MaKH = customerId,
                    HoTen = string.Empty,
                    SODT = string.Empty,
                    CCCD = string.Empty,
                    GPLX = string.Empty,
                    TrangThaiXacMinh = "Chưa gửi"
                };
                tbMaKhachHang.Text = $"KH{customerId:D3}";
            }
            catch (SqlException exception)
            {
                StatusText.Text = $"Không thể cấp mã khách hàng mới: {exception.Message}";
                return;
            }

            _isNewCustomer = true;
            CustomerGrid.SelectedItem = null;
            NameBox.Clear();
            PhoneBox.Clear();
            IdentityBox.Clear();
            LicenseBox.Clear();
            BlacklistButton.Content = "Đưa vào danh sách đen";
            _rentalHistory.Clear();
            HistoryHeading.Text = "Lịch sử thuê";
            StatusText.Text = "Nhập thông tin khách hàng mới rồi chọn Lưu hồ sơ.";
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer is null)
            {
                StatusText.Text = "Hãy tạo hồ sơ mới hoặc chọn một khách hàng.";
                return;
            }

            _selectedCustomer.HoTen = NameBox.Text.Trim();
            _selectedCustomer.SODT = PhoneBox.Text.Trim();
            _selectedCustomer.CCCD = IdentityBox.Text.Trim();
            _selectedCustomer.GPLX = LicenseBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(_selectedCustomer.HoTen)
                || string.IsNullOrWhiteSpace(_selectedCustomer.SODT)
                || string.IsNullOrWhiteSpace(_selectedCustomer.CCCD))
            {
                StatusText.Text = "Họ tên, số điện thoại và CCCD là thông tin bắt buộc.";
                return;
            }

            try
            {
                var affectedRows = _isNewCustomer
                    ? _selectedCustomer.ThemKhachHang(_selectedCustomer)
                    : _selectedCustomer.CapNhatKhachHang(_selectedCustomer);

                if (affectedRows == 0)
                {
                    StatusText.Text = "Không có hồ sơ nào được lưu. Hãy tải lại danh sách và thử lại.";
                    return;
                }

                var savedCustomerId = _selectedCustomer.MaKH;
                _isNewCustomer = false;
                LoadCustomers(savedCustomerId);
                StatusText.Text = "Đã lưu hồ sơ khách hàng vào SQL Server.";
            }
            catch (SqlException exception)
            {
                StatusText.Text = $"Không thể lưu hồ sơ khách hàng: {exception.Message}";
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer is null || _selectedCustomer.MaKH == 0)
            {
                StatusText.Text = "Hãy chọn hồ sơ khách hàng cần xóa.";
                return;
            }

            var confirmation = MessageBox.Show(
                $"Bạn có chắc muốn xóa hồ sơ {_selectedCustomer.HoTen}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                var affectedRows = new KhachHang().XoaKhachHang(_selectedCustomer.MaKH);
                if (affectedRows == 0)
                {
                    StatusText.Text = "Hồ sơ khách hàng không còn tồn tại trong SQL Server.";
                    LoadCustomers();
                    return;
                }

                _selectedCustomer = null;
                _isNewCustomer = false;
                tbMaKhachHang.Clear();
                NameBox.Clear();
                PhoneBox.Clear();
                IdentityBox.Clear();
                LicenseBox.Clear();
                _rentalHistory.Clear();
                LoadCustomers();
                StatusText.Text = "Đã xóa hồ sơ khách hàng khỏi SQL Server.";
            }
            catch (SqlException exception)
            {
                StatusText.Text = $"Không thể xóa hồ sơ khách hàng: {exception.Message}";
            }
        }

        private void ToggleBlacklist_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer is null || _isNewCustomer)
            {
                StatusText.Text = "Hãy chọn một hồ sơ khách hàng đã lưu.";
                return;
            }

            _selectedCustomer.HoTen = NameBox.Text.Trim();
            _selectedCustomer.SODT = PhoneBox.Text.Trim();
            _selectedCustomer.CCCD = IdentityBox.Text.Trim();
            _selectedCustomer.GPLX = LicenseBox.Text.Trim();
            _selectedCustomer.IsBlacklisted = !_selectedCustomer.IsBlacklisted;
            BlacklistButton.Content = _selectedCustomer.IsBlacklisted
                ? "Gỡ khỏi danh sách đen"
                : "Đưa vào danh sách đen";
            SaveCustomer_Click(sender, e);
        }
     
    }
}

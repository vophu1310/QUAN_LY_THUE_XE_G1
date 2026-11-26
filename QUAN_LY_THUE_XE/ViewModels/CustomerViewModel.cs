using Microsoft.Data.SqlClient;
using QUAN_LY_THUE_XE.Helpers;
using QUAN_LY_THUE_XE.Models;
using QUAN_LY_THUE_XE.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QUAN_LY_THUE_XE.ViewModels
{
    /// <summary>
    /// ViewModel cho màn hình Quản lý Khách hàng.
    /// Kế thừa BaseViewModel để có INotifyPropertyChanged.
    /// Dùng RelayCommand để bind ICommand từ XAML.
    /// Gọi CustomerService để truy xuất SQL Server.
    /// </summary>
    public class CustomerViewModel : BaseViewModel
    {
        private readonly CustomerService _service = new();

        // ============================================================
        // PROPERTIES — bind vào XAML qua {Binding ...}
        // ============================================================

        // Danh sách khách hàng hiển thị trên DataGrid (trang hiện tại).
        private ObservableCollection<KhachHang> _danhSachKhachHang = [];
        public ObservableCollection<KhachHang> DanhSachKhachHang
        {
            get => _danhSachKhachHang;
            set => SetProperty(ref _danhSachKhachHang, value);
        }

        // Khách hàng đang được chọn trên DataGrid.
        private KhachHang? _selectedCustomer;
        public KhachHang? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value))
                {
                    // Cập nhật trạng thái nút xóa khi chọn/bỏ chọn khách.
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        // Từ khóa tìm kiếm — mỗi khi thay đổi sẽ tự động tìm lại.
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    // Reload danh sách mỗi khi người dùng gõ vào ô tìm kiếm.
                    LoadCustomers();
                }
            }
        }

        // Tổng số khách trong kết quả tìm kiếm hiện tại.
        private int _tongSoKhachHang;
        public int TongSoKhachHang
        {
            get => _tongSoKhachHang;
            set => SetProperty(ref _tongSoKhachHang, value);
        }

        // Hai số liệu này được tính từ trạng thái danh sách đen đã lưu trong dữ liệu khách hàng.
        private int _soKhachAnToan;
        public int SoKhachAnToan
        {
            get => _soKhachAnToan;
            set => SetProperty(ref _soKhachAnToan, value);
        }

        private int _soKhachCanhBao;
        public int SoKhachCanhBao
        {
            get => _soKhachCanhBao;
            set => SetProperty(ref _soKhachCanhBao, value);
        }

        // 0 = tất cả, 1 = an toàn, 2 = danh sách đen; thứ tự tương ứng ComboBox trong CustomerView.xaml.
        private int _statusFilterIndex;
        public int StatusFilterIndex
        {
            get => _statusFilterIndex;
            set
            {
                if (SetProperty(ref _statusFilterIndex, value))
                {
                    _trangHienTai = 1;
                    OnPropertyChanged(nameof(TrangHienTai));
                    CapNhatTongSoTrang();
                    HienThiTrang();
                    OnPropertyChanged(nameof(ThongTinTrang));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        // Thông báo trạng thái (lỗi hoặc thành công).
        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ============================================================
        // PHÂN TRANG
        // ============================================================

        private const int SoKhachMoiTrang = 10;
        private List<KhachHang> _tatCaKhachHang = []; // toàn bộ dữ liệu
        private int _trangHienTai = 1;

        public int TrangHienTai
        {
            get => _trangHienTai;
            set
            {
                if (SetProperty(ref _trangHienTai, value))
                {
                    HienThiTrang();
                    OnPropertyChanged(nameof(ThongTinTrang));
                }
            }
        }

        private int _tongSoTrang = 1;
        public int TongSoTrang
        {
            get => _tongSoTrang;
            set => SetProperty(ref _tongSoTrang, value);
        }

        // Chuỗi hiển thị "Showing 1–10 of 93 entries".
        public string ThongTinTrang
        {
            get
            {
                int total = LayDanhSachTheoBoLoc().Count;
                int start = (_trangHienTai - 1) * SoKhachMoiTrang + 1;
                int end = Math.Min(_trangHienTai * SoKhachMoiTrang, total);
                if (total == 0) return "Không có dữ liệu";
                return $"Hiển thị {start}–{end} / {total} khách hàng";
            }
        }

        // ============================================================
        // COMMANDS — bind vào nút XAML qua {Binding ...Command}
        // ============================================================

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public CustomerViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadCustomers());
            AddCommand = new RelayCommand(_ => MoDialogThemKhach());
            DeleteCommand = new RelayCommand(_ => XoaKhachHang(), _ => SelectedCustomer != null);
            PreviousPageCommand = new RelayCommand(_ => TrangHienTai--, _ => TrangHienTai > 1);
            NextPageCommand = new RelayCommand(_ => TrangHienTai++, _ => TrangHienTai < TongSoTrang);

            // Load dữ liệu ngay khi ViewModel được tạo.
            LoadCustomers();
        }

        // ============================================================
        // METHODS
        // ============================================================

        /// <summary>
        /// Load toàn bộ hoặc tìm kiếm khách hàng từ SQL Server,
        /// sau đó phân trang và hiển thị.
        /// </summary>
        public void LoadCustomers()
        {
            try
            {
                // Lấy danh sách qua Service; truy vấn thực tế nằm ở Model và KHProvider.
                _tatCaKhachHang = string.IsNullOrWhiteSpace(_searchText)
                    ? _service.LayTatCaKhachHang()
                    : _service.TimKiemKhachHang(_searchText);

                TongSoKhachHang = _tatCaKhachHang.Count;
                SoKhachAnToan = _tatCaKhachHang.Count(customer => !customer.IsBlacklisted);
                SoKhachCanhBao = _tatCaKhachHang.Count(customer => customer.IsBlacklisted);

                // Khi tải/tìm kiếm lại, bắt đầu từ trang đầu với trạng thái lọc hiện hành.
                _trangHienTai = 1;
                OnPropertyChanged(nameof(TrangHienTai));
                CapNhatTongSoTrang();
                HienThiTrang();
                OnPropertyChanged(nameof(ThongTinTrang));

                StatusMessage = $"Đã tải {_tatCaKhachHang.Count} khách hàng từ SQL Server.";
            }
            catch (SqlException ex)
            {
                StatusMessage = $"Lỗi kết nối SQL Server: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                StatusMessage = $"Lỗi truy vấn dữ liệu: {ex.Message}";
            }
        }

        /// <summary>
        /// Áp dụng bộ lọc trạng thái đang chọn lên danh sách đã tải từ service.
        /// </summary>
        private List<KhachHang> LayDanhSachTheoBoLoc()
        {
            // Mã trạng thái được đồng bộ với thứ tự ComboBox: tất cả, an toàn, cảnh báo.
            return _statusFilterIndex switch
            {
                1 => _tatCaKhachHang.Where(customer => !customer.IsBlacklisted).ToList(),
                2 => _tatCaKhachHang.Where(customer => customer.IsBlacklisted).ToList(),
                _ => _tatCaKhachHang
            };
        }

        // Tính tổng trang dựa trên tập dữ liệu sau khi áp dụng bộ lọc trạng thái.
        private void CapNhatTongSoTrang()
        {
            int filteredCount = LayDanhSachTheoBoLoc().Count;
            TongSoTrang = Math.Max(1, (int)Math.Ceiling((double)filteredCount / SoKhachMoiTrang));
        }

        // Chỉ đưa tối đa SoKhachMoiTrang bản ghi của trang hiện tại lên DataGrid.
        private void HienThiTrang()
        {
            var trang = LayDanhSachTheoBoLoc()
                .Skip((_trangHienTai - 1) * SoKhachMoiTrang)
                .Take(SoKhachMoiTrang)
                .ToList();

            DanhSachKhachHang = new ObservableCollection<KhachHang>(trang);
        }

        /// <summary>
        /// Mở popup AddCustomerDialog để nhập thông tin khách hàng mới.
        /// Sau khi dialog đóng (OK), lưu vào SQL Server và reload danh sách.
        /// </summary>
        private void MoDialogThemKhach()
        {
            try
            {
                int maKHMoi = _service.LayMaKHKeTiep();
                var khachMoi = new KhachHang
                {
                    MaKH = maKHMoi,
                    HoTen = string.Empty,
                    SODT = string.Empty,
                    CCCD = string.Empty,
                    GPLX = string.Empty,
                    TrangThaiXacMinh = "Chưa gửi"
                };

                // Mở dialog, truyền đối tượng khách mới vào.
                var dialog = new Views.AddCustomerDialog(khachMoi);
                if (dialog.ShowDialog() == true)
                {
                    // Người dùng bấm Lưu → ghi vào SQL Server.
                    int rows = _service.ThemKhachHang(dialog.KhachHangMoi);
                    if (rows > 0)
                    {
                        StatusMessage = $"Đã thêm khách hàng {dialog.KhachHangMoi.HoTen} thành công.";
                        LoadCustomers();
                    }
                    else
                    {
                        StatusMessage = "Không thể thêm khách hàng. Vui lòng thử lại.";
                    }
                }
            }
            catch (SqlException ex)
            {
                StatusMessage = $"Lỗi thêm khách hàng: {ex.Message}";
            }
        }

        /// <summary>
        /// Xóa khách hàng đang được chọn trên DataGrid.
        /// </summary>
        private void XoaKhachHang()
        {
            // Không gửi lệnh SQL nếu chưa có khách được chọn hoặc người dùng hủy xác nhận.
            if (SelectedCustomer is null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa khách hàng {SelectedCustomer.HoTen} (KH{SelectedCustomer.MaKH:D3})?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                int rows = _service.XoaKhachHang(SelectedCustomer.MaKH);
                if (rows > 0)
                {
                    StatusMessage = $"Đã xóa khách hàng {SelectedCustomer.HoTen}.";
                    SelectedCustomer = null;
                    LoadCustomers();
                }
                else
                {
                    StatusMessage = "Khách hàng không còn tồn tại trong cơ sở dữ liệu.";
                    LoadCustomers();
                }
            }
            catch (SqlException ex)
            {
                StatusMessage = $"Lỗi xóa khách hàng: {ex.Message}";
            }
        }
    }
}


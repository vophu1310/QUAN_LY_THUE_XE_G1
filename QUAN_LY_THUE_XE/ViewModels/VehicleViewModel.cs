using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QUAN_LY_THUE_XE.Helpers;
using QUAN_LY_THUE_XE.Models;
using QUAN_LY_THUE_XE.Services;
using QUAN_LY_THUE_XE.Views;

namespace QUAN_LY_THUE_XE.ViewModels
{
    /// <summary>
    /// ViewModel cho màn hình Quản lý Đội xe (VehicleView).
    /// Chỉ dùng OnPropertyChanged() từ BaseViewModel để tương thích 100% với nhánh master.
    /// </summary>
    public class VehicleViewModel : BaseViewModel
    {
        private readonly VehicleService _service = new();
        private const int SoXeMoiTrang = 8;

        private List<Xe> _tatCaXe = new();

        // ============================================================
        // DANH SÁCH HIỂN THỊ & PHÂN TRANG
        // ============================================================

        private ObservableCollection<Xe> _danhSachXeHienThi = new();
        public ObservableCollection<Xe> DanhSachXeHienThi
        {
            get => _danhSachXeHienThi;
            set
            {
                _danhSachXeHienThi = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(KhongCoDuLieu));
            }
        }

        public bool KhongCoDuLieu => DanhSachXeHienThi.Count == 0;

        private int _trangHienTai = 1;
        public int TrangHienTai
        {
            get => _trangHienTai;
            set
            {
                if (_trangHienTai == value) return;
                _trangHienTai = value;
                OnPropertyChanged();
                CapNhatTrangHienTai();
            }
        }

        private int _tongSoTrang = 1;
        public int TongSoTrang
        {
            get => _tongSoTrang;
            set
            {
                if (_tongSoTrang == value) return;
                _tongSoTrang = value;
                OnPropertyChanged();
            }
        }

        private string _thongTinTrang = "Không có dữ liệu";
        public string ThongTinTrang
        {
            get => _thongTinTrang;
            set
            {
                if (_thongTinTrang == value) return;
                _thongTinTrang = value;
                OnPropertyChanged();
            }
        }

        // ============================================================
        // CHỈ SỐ THỐNG KÊ HEADER (3 THẺ FIGMA)
        // ============================================================

        private int _tongSoXe;
        public int TongSoXe
        {
            get => _tongSoXe;
            set { if (_tongSoXe == value) return; _tongSoXe = value; OnPropertyChanged(); }
        }

        private int _soXeSanSang;
        public int SoXeSanSang
        {
            get => _soXeSanSang;
            set { if (_soXeSanSang == value) return; _soXeSanSang = value; OnPropertyChanged(); }
        }

        private int _soXeDangThue;
        public int SoXeDangThue
        {
            get => _soXeDangThue;
            set { if (_soXeDangThue == value) return; _soXeDangThue = value; OnPropertyChanged(); }
        }

        private int _soXeBaoDuong;
        public int SoXeBaoDuong
        {
            get => _soXeBaoDuong;
            set { if (_soXeBaoDuong == value) return; _soXeBaoDuong = value; OnPropertyChanged(); }
        }

        // ============================================================
        // TÌM KIẾM & BỘ LỌC (PHÂN KHÚC + TRẠNG THÁI)
        // ============================================================

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
                ApDungBoLocVaPhanTrang(resetVeTrangDau: true);
            }
        }

        // "Tất cả", "4 chỗ", "5 chỗ", "7 chỗ", "SUV", "Xe điện"
        private string _selectedCategory = "Tất cả";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                OnPropertyChanged();
                ApDungBoLocVaPhanTrang(resetVeTrangDau: true);
            }
        }

        // 0: Tất cả trạng thái, 1: Sẵn sàng, 2: Đang thuê, 3: Đang bảo dưỡng, 4: Ngưng hoạt động
        private int _statusFilterIndex;
        public int StatusFilterIndex
        {
            get => _statusFilterIndex;
            set
            {
                if (_statusFilterIndex == value) return;
                _statusFilterIndex = value;
                OnPropertyChanged();
                ApDungBoLocVaPhanTrang(resetVeTrangDau: true);
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage == value) return;
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        // ============================================================
        // COMMANDS
        // ============================================================

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DetailCommand { get; }
        public ICommand ToggleMaintenanceCommand { get; }
        public ICommand SoftDeleteCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public VehicleViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadVehicles());
            AddCommand = new RelayCommand(_ => MoDialogThemXe());
            EditCommand = new RelayCommand(p => MoDialogSuaXe(p as Xe), p => p is Xe);
            DetailCommand = new RelayCommand(p => MoDialogChiTietXe(p as Xe), p => p is Xe);
            ToggleMaintenanceCommand = new RelayCommand(p => ChuyenBaoDuong(p as Xe), p => p is Xe);
            SoftDeleteCommand = new RelayCommand(p => XoaMemHoacKhoiPhucXe(p as Xe), p => p is Xe);
            SelectCategoryCommand = new RelayCommand(p =>
            {
                if (p is string category && !string.IsNullOrWhiteSpace(category))
                {
                    SelectedCategory = category;
                }
            });
            ResetFilterCommand = new RelayCommand(_ => DatLaiBoLoc());
            PreviousPageCommand = new RelayCommand(_ => TrangHienTai--, _ => TrangHienTai > 1);
            NextPageCommand = new RelayCommand(_ => TrangHienTai++, _ => TrangHienTai < TongSoTrang);

            LoadVehicles();
        }

        // ============================================================
        // XỬ LÝ DỮ LIỆU & NGHIỆP VỤ
        // ============================================================

        public void LoadVehicles()
        {
            try
            {
                _tatCaXe = _service.LayTatCaXe();

                TongSoXe = _tatCaXe.Count;
                SoXeSanSang = _tatCaXe.Count(x => x.TrangThai == VehicleService.TrangThaiSanSang);
                SoXeDangThue = _tatCaXe.Count(x => VehicleService.LaTrangThaiDangBan(x.TrangThai));
                SoXeBaoDuong = _tatCaXe.Count(x => x.TrangThai == VehicleService.TrangThaiBaoDuong);

                ApDungBoLocVaPhanTrang(resetVeTrangDau: false);
                StatusMessage = $"Đã tải {_tatCaXe.Count} phương tiện từ SQL Server (Sẵn sàng: {SoXeSanSang} · Đang thuê: {SoXeDangThue} · Bảo dưỡng: {SoXeBaoDuong}).";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi tải danh sách xe: {ex.Message}";
            }
        }

        private List<Xe> LayDanhSachSauBoLoc()
        {
            IEnumerable<Xe> query = _tatCaXe;

            // 1. Lọc theo từ khóa (biển số, tên xe, hãng xe)
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                string keyword = _searchText.Trim();
                query = query.Where(x =>
                    (!string.IsNullOrEmpty(x.BienSo) && x.BienSo.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrEmpty(x.TenXe) && x.TenXe.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrEmpty(x.HangXe) && x.HangXe.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            // 2. Lọc theo phân khúc (Chips Figma)
            query = _selectedCategory switch
            {
                "4 chỗ" => query.Where(x => x.SoChoNgoi <= 4),
                "5 chỗ" => query.Where(x => x.SoChoNgoi == 5),
                "7 chỗ" => query.Where(x => x.SoChoNgoi >= 7),
                "SUV" => query.Where(LaXeSUV),
                "Xe điện" => query.Where(LaXeDien),
                _ => query
            };

            // 3. Lọc theo trạng thái
            query = _statusFilterIndex switch
            {
                1 => query.Where(x => x.TrangThai == VehicleService.TrangThaiSanSang),
                2 => query.Where(x => VehicleService.LaTrangThaiDangBan(x.TrangThai)),
                3 => query.Where(x => x.TrangThai == VehicleService.TrangThaiBaoDuong),
                4 => query.Where(x => x.TrangThai == VehicleService.TrangThaiNgungHoatDong),
                _ => query
            };

            return query.ToList();
        }

        private static bool LaXeSUV(Xe xe)
        {
            string ten = xe.TenXe ?? string.Empty;
            string[] tuKhoaSuv =
            {
                "SUV", "CX-5", "CX5", "CX-8", "Everest", "Santa Fe", "SantaFe",
                "Fortuner", "VF8", "VF 8", "VF9", "VF 9", "VF6", "VF7",
                "RAV4", "CR-V", "CRV", "Tucson", "Seltos", "Sonet", "Territory",
                "Outlander", "X-Trail", "Range Rover", "Model Y"
            };
            return xe.SoChoNgoi >= 7 || tuKhoaSuv.Any(k => ten.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        private static bool LaXeDien(Xe xe)
        {
            string hang = xe.HangXe ?? string.Empty;
            string ten = xe.TenXe ?? string.Empty;
            return hang.Contains("VinFast", StringComparison.OrdinalIgnoreCase)
                || hang.Contains("Tesla", StringComparison.OrdinalIgnoreCase)
                || hang.Contains("BYD", StringComparison.OrdinalIgnoreCase)
                || ten.Contains("VF", StringComparison.OrdinalIgnoreCase)
                || ten.Contains("Electric", StringComparison.OrdinalIgnoreCase)
                || ten.Contains("Điện", StringComparison.OrdinalIgnoreCase)
                || ten.Contains("EV", StringComparison.OrdinalIgnoreCase);
        }

        private void ApDungBoLocVaPhanTrang(bool resetVeTrangDau)
        {
            var filtered = LayDanhSachSauBoLoc();
            TongSoTrang = Math.Max(1, (int)Math.Ceiling((double)filtered.Count / SoXeMoiTrang));

            if (resetVeTrangDau || _trangHienTai > TongSoTrang)
            {
                _trangHienTai = 1;
                OnPropertyChanged(nameof(TrangHienTai));
            }

            CapNhatTrangHienTai(filtered);
            CommandManager.InvalidateRequerySuggested();
        }

        private void CapNhatTrangHienTai(List<Xe>? prefiltered = null)
        {
            var filtered = prefiltered ?? LayDanhSachSauBoLoc();
            int total = filtered.Count;

            var pageItems = filtered
                .Skip((_trangHienTai - 1) * SoXeMoiTrang)
                .Take(SoXeMoiTrang)
                .ToList();

            DanhSachXeHienThi = new ObservableCollection<Xe>(pageItems);

            if (total == 0)
            {
                ThongTinTrang = "Không có phương tiện phù hợp";
            }
            else
            {
                int start = (_trangHienTai - 1) * SoXeMoiTrang + 1;
                int end = Math.Min(_trangHienTai * SoXeMoiTrang, total);
                ThongTinTrang = $"Hiển thị {start}–{end} / {total} phương tiện";
            }
        }

        private void DatLaiBoLoc()
        {
            _searchText = string.Empty;
            _selectedCategory = "Tất cả";
            _statusFilterIndex = 0;
            OnPropertyChanged(nameof(SearchText));
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(StatusFilterIndex));
            ApDungBoLocVaPhanTrang(resetVeTrangDau: true);
        }

        private void MoDialogThemXe()
        {
            var dialog = new VehicleDialog(null, VehicleDialogMode.Add)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.KetQuaXe != null)
            {
                if (_service.ThemXe(dialog.KetQuaXe, out string msg))
                {
                    LoadVehicles();
                    StatusMessage = msg;
                    MessageBox.Show(msg, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    StatusMessage = msg;
                    MessageBox.Show(msg, "Không thể thêm xe", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void MoDialogSuaXe(Xe? xe)
        {
            if (xe == null) return;

            var dialog = new VehicleDialog(xe, VehicleDialogMode.Edit)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.KetQuaXe != null)
            {
                if (_service.CapNhatXe(dialog.KetQuaXe, out string msg))
                {
                    LoadVehicles();
                    StatusMessage = msg;
                    MessageBox.Show(msg, "Cập nhật thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    StatusMessage = msg;
                    MessageBox.Show(msg, "Không thể cập nhật", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void MoDialogChiTietXe(Xe? xe)
        {
            if (xe == null) return;

            var dialog = new VehicleDialog(xe, VehicleDialogMode.Detail)
            {
                Owner = Application.Current.MainWindow
            };

            dialog.ShowDialog();

            // Nếu người dùng bấm "Chỉnh sửa" từ màn hình chi tiết
            if (dialog.YeuCauChuyenSangSua)
            {
                MoDialogSuaXe(xe);
            }
        }

        private void ChuyenBaoDuong(Xe? xe)
        {
            if (xe == null) return;

            string hanhDong = xe.TrangThai == VehicleService.TrangThaiBaoDuong
                ? $"xác nhận hoàn tất bảo dưỡng cho xe {xe.TenXe} ({xe.BienSo}) và đưa về trạng thái 'Sẵn sàng'"
                : $"chuyển xe {xe.TenXe} ({xe.BienSo}) sang trạng thái 'Đang bảo dưỡng'";

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn {hanhDong}?",
                "Xác nhận trạng thái bảo dưỡng",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            if (_service.ChuyenTrangThaiBaoDuong(xe.MaXe, out string msg))
            {
                LoadVehicles();
                StatusMessage = msg;
            }
            else
            {
                StatusMessage = msg;
                MessageBox.Show(msg, "Thông báo nghiệp vụ", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void XoaMemHoacKhoiPhucXe(Xe? xe)
        {
            if (xe == null) return;

            bool dangNgung = xe.TrangThai == VehicleService.TrangThaiNgungHoatDong;
            string tieuDe = dangNgung ? "Khôi phục phương tiện" : "Ngưng hoạt động phương tiện (Xóa mềm)";
            string noiDung = dangNgung
                ? $"Khôi phục xe {xe.TenXe} ({xe.BienSo}) về trạng thái 'Sẵn sàng' để tiếp tục cho thuê?"
                : $"Chuyển xe {xe.TenXe} ({xe.BienSo}) sang trạng thái 'Ngưng hoạt động'?\n(Dữ liệu lịch sử thuê xe vẫn được giữ nguyên theo quy tắc BR-01)";

            var confirm = MessageBox.Show(
                noiDung,
                tieuDe,
                MessageBoxButton.YesNo,
                dangNgung ? MessageBoxImage.Question : MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            if (_service.XoaMemXe(xe.MaXe, out string msg))
            {
                LoadVehicles();
                StatusMessage = msg;
            }
            else
            {
                StatusMessage = msg;
                MessageBox.Show(msg, "Không thể thực hiện (BR-01)", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

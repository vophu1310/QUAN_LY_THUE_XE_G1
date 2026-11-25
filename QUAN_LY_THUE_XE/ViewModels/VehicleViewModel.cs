using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using QUAN_LY_THUE_XE.Helpers;
using QUAN_LY_THUE_XE.Models;
using QUAN_LY_THUE_XE.Services;

namespace QUAN_LY_THUE_XE.ViewModels
{
    public class VehicleViewModel : BaseViewModel
    {
        private readonly VehicleService _vehicleService;
        private List<Xe> _tatCaXe = new();
        private List<Xe> _xeSauLoc = new();
        private const int KichThuocTrang = 8; // Lưới 4 cột x 2 hàng chuẩn Figma

        private static readonly string[] TuKhoaSuv =
        {
            "SUV", "CX-5", "CX5", "CX-8", "SANTA FE", "EVEREST", "FORTUNER",
            "VF8", "VF 8", "VF9", "VF 9", "RAV4", "CR-V", "CRV", "TUCSON",
            "TERRITORY", "CRETA", "SELTOS", "X-TRAIL", "OUTLANDER", "PAJERO"
        };

        private static readonly string[] TuKhoaXeDien =
        {
            "VINFAST", "TESLA", "BYD", "WULING", "VF", "ELECTRIC", "EV", "ĐIỆN"
        };

        public ObservableCollection<Xe> DanhSachXeHienThi { get; } = new();

        public List<string> DanhSachTrangThai { get; } = new()
        {
            "Sẵn sàng",
            "Đang thuê",
            "Đang bảo dưỡng",
            "Ngưng hoạt động"
        };

        public List<string> DanhSachHangXeGoiY { get; } = new()
        {
            "Toyota", "Mazda", "Ford", "VinFast", "Hyundai", "Kia", "Mitsubishi", "Honda"
        };

        public List<string> DanhSachSoChoGoiY { get; } = new()
        {
            "4", "5", "6", "7", "16"
        };

        // ================= 1. KPI TỔNG QUAN =================
        private int _tongSoXe;
        public int TongSoXe
        {
            get => _tongSoXe;
            set { if (SetProperty(ref _tongSoXe, value)) OnPropertyChanged(nameof(TongSoXeText)); }
        }
        public string TongSoXeText => TongSoXe.ToString("D2");

        private int _soXeSanSang;
        public int SoXeSanSang
        {
            get => _soXeSanSang;
            set { if (SetProperty(ref _soXeSanSang, value)) OnPropertyChanged(nameof(SoXeSanSangText)); }
        }
        public string SoXeSanSangText => SoXeSanSang.ToString("D2");

        private int _soXeDangThue;
        public int SoXeDangThue
        {
            get => _soXeDangThue;
            set { if (SetProperty(ref _soXeDangThue, value)) OnPropertyChanged(nameof(SoXeDangThueText)); }
        }
        public string SoXeDangThueText => SoXeDangThue.ToString("D2");

        // ================= 2. TÌM KIẾM, BỘ LỌC & PHÂN TRANG =================
        private string _tuKhoaTimKiem = string.Empty;
        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set
            {
                if (SetProperty(ref _tuKhoaTimKiem, value))
                {
                    TrangHienTai = 1;
                    ApDungBoLocVaPhanTrang();
                }
            }
        }

        private string _boLocDangChon = "Tất cả";
        public string BoLocDangChon
        {
            get => _boLocDangChon;
            set
            {
                if (SetProperty(ref _boLocDangChon, value))
                {
                    TrangHienTai = 1;
                    ApDungBoLocVaPhanTrang();
                }
            }
        }

        private string _locTrangThaiKpi = string.Empty;
        public string LocTrangThaiKpi
        {
            get => _locTrangThaiKpi;
            set
            {
                if (SetProperty(ref _locTrangThaiKpi, value))
                {
                    TrangHienTai = 1;
                    ApDungBoLocVaPhanTrang();
                }
            }
        }

        private int _trangHienTai = 1;
        public int TrangHienTai
        {
            get => _trangHienTai;
            set
            {
                if (SetProperty(ref _trangHienTai, value))
                {
                    OnPropertyChanged(nameof(ThongTinHienThi));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private int _tongSoTrang = 1;
        public int TongSoTrang
        {
            get => _tongSoTrang;
            set
            {
                if (SetProperty(ref _tongSoTrang, value))
                {
                    OnPropertyChanged(nameof(ThongTinHienThi));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string ThongTinHienThi
        {
            get
            {
                if (_xeSauLoc.Count == 0) return "Không có phương tiện phù hợp";
                string locTrangThai = string.IsNullOrEmpty(LocTrangThaiKpi) ? string.Empty : $" • Trạng thái: {LocTrangThaiKpi}";
                return $"Hiển thị {DanhSachXeHienThi.Count} / {_xeSauLoc.Count} xe{locTrangThai}";
            }
        }

        private bool _khongCoDuLieu;
        public bool KhongCoDuLieu
        {
            get => _khongCoDuLieu;
            set => SetProperty(ref _khongCoDuLieu, value);
        }

        private string _thongBaoHeThong = string.Empty;
        public string ThongBaoHeThong
        {
            get => _thongBaoHeThong;
            set => SetProperty(ref _thongBaoHeThong, value);
        }

        // ================= 3. MODAL THÊM / SỬA XE =================
        private bool _dangMoForm;
        public bool DangMoForm
        {
            get => _dangMoForm;
            set => SetProperty(ref _dangMoForm, value);
        }

        private bool _laCheDoSua;
        public bool LaCheDoSua
        {
            get => _laCheDoSua;
            set
            {
                if (SetProperty(ref _laCheDoSua, value))
                    OnPropertyChanged(nameof(TieuDeForm));
            }
        }

        public string TieuDeForm => LaCheDoSua ? "Cập nhật thông tin xe" : "Thêm phương tiện mới";

        private int _formMaXe;
        private string _formBienSo = string.Empty;
        public string FormBienSo { get => _formBienSo; set => SetProperty(ref _formBienSo, value); }

        private string _formTenXe = string.Empty;
        public string FormTenXe { get => _formTenXe; set => SetProperty(ref _formTenXe, value); }

        private string _formHangXe = string.Empty;
        public string FormHangXe { get => _formHangXe; set => SetProperty(ref _formHangXe, value); }

        private string _formNamSanXuat = DateTime.Now.Year.ToString();
        public string FormNamSanXuat { get => _formNamSanXuat; set => SetProperty(ref _formNamSanXuat, value); }

        private string _formSoChoNgoi = "5";
        public string FormSoChoNgoi { get => _formSoChoNgoi; set => SetProperty(ref _formSoChoNgoi, value); }

        private string _formGiaThueNgay = string.Empty;
        public string FormGiaThueNgay { get => _formGiaThueNgay; set => SetProperty(ref _formGiaThueNgay, value); }

        private string _formTienCocMacDinh = "0";
        public string FormTienCocMacDinh { get => _formTienCocMacDinh; set => SetProperty(ref _formTienCocMacDinh, value); }

        private string _formSoKmHienTai = "0";
        public string FormSoKmHienTai { get => _formSoKmHienTai; set => SetProperty(ref _formSoKmHienTai, value); }

        private string _formTrangThai = "Sẵn sàng";
        public string FormTrangThai { get => _formTrangThai; set => SetProperty(ref _formTrangThai, value); }

        private string _formHinhAnh = string.Empty;
        public string FormHinhAnh { get => _formHinhAnh; set => SetProperty(ref _formHinhAnh, value); }

        private string _thongBaoLoiForm = string.Empty;
        public string ThongBaoLoiForm { get => _thongBaoLoiForm; set => SetProperty(ref _thongBaoLoiForm, value); }

        // ================= 4. MODAL CHI TIẾT XE =================
        private bool _dangMoChiTiet;
        public bool DangMoChiTiet
        {
            get => _dangMoChiTiet;
            set => SetProperty(ref _dangMoChiTiet, value);
        }

        private Xe? _xeChiTiet;
        public Xe? XeChiTiet
        {
            get => _xeChiTiet;
            set
            {
                if (SetProperty(ref _xeChiTiet, value))
                    OnPropertyChanged(nameof(NutNgungHoatDongText));
            }
        }

        public string NutNgungHoatDongText =>
            XeChiTiet?.TrangThai == "Ngưng hoạt động" ? "Mở lại Sẵn sàng" : "Ngưng hoạt động";

        // ================= 5. COMMANDS =================
        public ICommand ChonBoLocCommand { get; }
        public ICommand ChonLocKpiCommand { get; }
        public ICommand LamMoiCommand { get; }
        public ICommand TrangTruocCommand { get; }
        public ICommand TrangSauCommand { get; }

        public ICommand MoFormThemCommand { get; }
        public ICommand MoFormSuaCommand { get; }
        public ICommand DongFormCommand { get; }
        public ICommand LuuXeCommand { get; }
        public ICommand ChonAnhCommand { get; }
        public ICommand XoaAnhCommand { get; }

        public ICommand MoChiTietCommand { get; }
        public ICommand DongChiTietCommand { get; }
        public ICommand SuaTuChiTietCommand { get; }
        public ICommand XoaXeCommand { get; }
        public ICommand ChuyenTrangThaiNgungCommand { get; }

        public VehicleViewModel()
        {
            _vehicleService = new VehicleService();

            ChonBoLocCommand = new RelayCommand(p =>
            {
                if (p is string filter)
                {
                    if (filter == "Tất cả") _locTrangThaiKpi = string.Empty;
                    OnPropertyChanged(nameof(LocTrangThaiKpi));
                    BoLocDangChon = filter;
                }
            });

            ChonLocKpiCommand = new RelayCommand(p =>
            {
                string trangThai = p?.ToString() ?? string.Empty;
                LocTrangThaiKpi = LocTrangThaiKpi == trangThai ? string.Empty : trangThai;
            });

            LamMoiCommand = new RelayCommand(_ =>
            {
                LoadDuLieu();
                ThongBaoHeThong = $"Đã đồng bộ dữ liệu lúc {DateTime.Now:HH:mm:ss}.";
            });

            TrangTruocCommand = new RelayCommand(
                _ => { TrangHienTai--; ApDungBoLocVaPhanTrang(); },
                _ => TrangHienTai > 1);

            TrangSauCommand = new RelayCommand(
                _ => { TrangHienTai++; ApDungBoLocVaPhanTrang(); },
                _ => TrangHienTai < TongSoTrang);

            MoFormThemCommand = new RelayCommand(_ => MoFormThemMoi());
            MoFormSuaCommand = new RelayCommand(p => { if (p is Xe xe) MoFormChinhSua(xe); });
            DongFormCommand = new RelayCommand(_ => DangMoForm = false);
            LuuXeCommand = new RelayCommand(_ => ThucHienLuuXe());
            ChonAnhCommand = new RelayCommand(_ => ThucHienChonAnh());
            XoaAnhCommand = new RelayCommand(_ => FormHinhAnh = string.Empty);

            MoChiTietCommand = new RelayCommand(p =>
            {
                if (p is Xe xe)
                {
                    XeChiTiet = xe;
                    DangMoChiTiet = true;
                }
            });

            DongChiTietCommand = new RelayCommand(_ => DangMoChiTiet = false);

            SuaTuChiTietCommand = new RelayCommand(_ =>
            {
                if (XeChiTiet != null)
                {
                    var xe = XeChiTiet;
                    DangMoChiTiet = false;
                    MoFormChinhSua(xe);
                }
            });

            XoaXeCommand = new RelayCommand(p => ThucHienXoaXe(p as Xe));
            ChuyenTrangThaiNgungCommand = new RelayCommand(_ => ThucHienChuyenNgungHoatDong());

            LoadDuLieu();
        }

        public void LoadDuLieu()
        {
            try
            {
                _tatCaXe = _vehicleService.LayTatCaXe();
                var (tong, sanSang, dangThue) = _vehicleService.LayThongKeXe();
                TongSoXe = tong;
                SoXeSanSang = sanSang;
                SoXeDangThue = dangThue;

                ApDungBoLocVaPhanTrang();
            }
            catch (Exception ex)
            {
                ThongBaoHeThong = $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}";
            }
        }

        private void ApDungBoLocVaPhanTrang()
        {
            IEnumerable<Xe> query = _tatCaXe;

            // 1. Tìm kiếm nhanh theo biển số, tên xe hoặc hãng xe
            if (!string.IsNullOrWhiteSpace(TuKhoaTimKiem))
            {
                string kw = TuKhoaTimKiem.Trim();
                query = query.Where(x =>
                    (!string.IsNullOrEmpty(x.BienSo) && x.BienSo.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(x.TenXe) && x.TenXe.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(x.HangXe) && x.HangXe.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            // 2. Lọc theo thẻ KPI nếu đang bật (Sẵn sàng / Đang thuê)
            if (!string.IsNullOrEmpty(LocTrangThaiKpi))
            {
                query = LocTrangThaiKpi == "Đang thuê"
                    ? query.Where(x => x.TrangThai == "Đang thuê" || x.TrangThai == "Đang cho thuê")
                    : query.Where(x => x.TrangThai == LocTrangThaiKpi);
            }

            // 3. Lọc theo 5 nút viên thuốc chuẩn Figma
            query = BoLocDangChon switch
            {
                "4 chỗ" => query.Where(x => x.SoChoNgoi <= 5),
                "7 chỗ" => query.Where(x => x.SoChoNgoi >= 7),
                "SUV" => query.Where(LaXeSuv),
                "Xe điện" => query.Where(LaXeDien),
                _ => query
            };

            _xeSauLoc = query.ToList();
            TongSoTrang = Math.Max(1, (int)Math.Ceiling(_xeSauLoc.Count / (double)KichThuocTrang));
            if (TrangHienTai > TongSoTrang) TrangHienTai = TongSoTrang;

            var dsTrang = _xeSauLoc
                .Skip((TrangHienTai - 1) * KichThuocTrang)
                .Take(KichThuocTrang)
                .ToList();

            DanhSachXeHienThi.Clear();
            foreach (var xe in dsTrang)
            {
                DanhSachXeHienThi.Add(xe);
            }

            KhongCoDuLieu = DanhSachXeHienThi.Count == 0;
            OnPropertyChanged(nameof(ThongTinHienThi));
        }

        private static bool LaXeSuv(Xe xe)
        {
            string ten = (xe.TenXe ?? string.Empty).ToUpperInvariant();
            return TuKhoaSuv.Any(k => ten.Contains(k));
        }

        private static bool LaXeDien(Xe xe)
        {
            string gop = $"{xe.HangXe} {xe.TenXe}".ToUpperInvariant();
            return TuKhoaXeDien.Any(k => gop.Contains(k));
        }

        private void MoFormThemMoi()
        {
            LaCheDoSua = false;
            _formMaXe = 0;
            FormBienSo = string.Empty;
            FormTenXe = string.Empty;
            FormHangXe = "Toyota";
            FormNamSanXuat = DateTime.Now.Year.ToString();
            FormSoChoNgoi = "5";
            FormGiaThueNgay = string.Empty;
            FormTienCocMacDinh = "0";
            FormSoKmHienTai = "0";
            FormTrangThai = "Sẵn sàng";
            FormHinhAnh = string.Empty;
            ThongBaoLoiForm = string.Empty;
            DangMoForm = true;
        }

        private void MoFormChinhSua(Xe xe)
        {
            LaCheDoSua = true;
            _formMaXe = xe.MaXe;
            FormBienSo = xe.BienSo ?? string.Empty;
            FormTenXe = xe.TenXe ?? string.Empty;
            FormHangXe = xe.HangXe ?? string.Empty;
            FormNamSanXuat = xe.NamSanXuat > 0 ? xe.NamSanXuat.ToString() : DateTime.Now.Year.ToString();
            FormSoChoNgoi = xe.SoChoNgoi.ToString();
            FormGiaThueNgay = xe.GiaThueNgay.ToString("0", CultureInfo.InvariantCulture);
            FormTienCocMacDinh = xe.TienCocMacDinh.ToString("0", CultureInfo.InvariantCulture);
            FormSoKmHienTai = xe.SoKmHienTai.ToString();
            FormTrangThai = string.IsNullOrWhiteSpace(xe.TrangThai) ? "Sẵn sàng" : xe.TrangThai;
            FormHinhAnh = xe.HinhAnh ?? string.Empty;
            ThongBaoLoiForm = string.Empty;
            DangMoForm = true;
        }

        private void ThucHienLuuXe()
        {
            ThongBaoLoiForm = string.Empty;

            if (string.IsNullOrWhiteSpace(FormBienSo))
            {
                ThongBaoLoiForm = "Vui lòng nhập biển số xe (VD: 51K-123.45).";
                return;
            }

            if (string.IsNullOrWhiteSpace(FormTenXe))
            {
                ThongBaoLoiForm = "Vui lòng nhập tên xe.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FormHangXe))
            {
                ThongBaoLoiForm = "Vui lòng nhập hoặc chọn hãng xe.";
                return;
            }

            if (!int.TryParse(ChuanHoaSo(FormSoChoNgoi), out int soCho) || soCho < 2 || soCho > 50)
            {
                ThongBaoLoiForm = "Số chỗ ngồi không hợp lệ (nhập số từ 2 đến 50).";
                return;
            }

            if (!int.TryParse(ChuanHoaSo(FormNamSanXuat), out int namSx) || namSx < 1990 || namSx > DateTime.Now.Year + 1)
            {
                ThongBaoLoiForm = $"Năm sản xuất không hợp lệ (1990 – {DateTime.Now.Year + 1}).";
                return;
            }

            if (!decimal.TryParse(ChuanHoaSo(FormGiaThueNgay), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal giaThue) || giaThue <= 0)
            {
                ThongBaoLoiForm = "Giá thuê/ngày phải là số lớn hơn 0.";
                return;
            }

            decimal tienCoc = 0;
            if (!string.IsNullOrWhiteSpace(FormTienCocMacDinh) &&
                (!decimal.TryParse(ChuanHoaSo(FormTienCocMacDinh), NumberStyles.Number, CultureInfo.InvariantCulture, out tienCoc) || tienCoc < 0))
            {
                ThongBaoLoiForm = "Tiền cọc mặc định không hợp lệ.";
                return;
            }

            int soKm = 0;
            if (!string.IsNullOrWhiteSpace(FormSoKmHienTai) &&
                (!int.TryParse(ChuanHoaSo(FormSoKmHienTai), out soKm) || soKm < 0))
            {
                ThongBaoLoiForm = "Số Km hiện tại không hợp lệ.";
                return;
            }

            var xe = new Xe
            {
                MaXe = _formMaXe,
                BienSo = FormBienSo.Trim(),
                TenXe = FormTenXe.Trim(),
                HangXe = FormHangXe.Trim(),
                NamSanXuat = namSx,
                SoChoNgoi = soCho,
                GiaThueNgay = giaThue,
                TienCocMacDinh = tienCoc,
                SoKmHienTai = soKm,
                TrangThai = FormTrangThai,
                HinhAnh = FormHinhAnh.Trim()
            };

            var (thanhCong, thongBao) = LaCheDoSua
                ? _vehicleService.CapNhatXe(xe)
                : _vehicleService.ThemXe(xe);

            if (!thanhCong)
            {
                ThongBaoLoiForm = thongBao;
                return;
            }

            DangMoForm = false;
            ThongBaoHeThong = thongBao;
            LoadDuLieu();
        }

        private void ThucHienXoaXe(Xe? xeTruyenVao)
        {
            var xeCanXoa = xeTruyenVao ?? XeChiTiet ?? _tatCaXe.FirstOrDefault(x => x.MaXe == _formMaXe);
            if (xeCanXoa == null) return;

            var xacNhan = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa xe {xeCanXoa.TenXe} ({xeCanXoa.BienSo}) khỏi hệ thống?",
                "Xác nhận xóa xe",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (xacNhan != MessageBoxResult.Yes) return;

            var (thanhCong, thongBao) = _vehicleService.XoaXe(xeCanXoa.MaXe);
            if (!thanhCong)
            {
                MessageBox.Show(thongBao, "Không thể xóa xe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DangMoForm = false;
            DangMoChiTiet = false;
            ThongBaoHeThong = thongBao;
            LoadDuLieu();
        }

        private void ThucHienChuyenNgungHoatDong()
        {
            if (XeChiTiet == null) return;

            string trangThaiMoi = XeChiTiet.TrangThai == "Ngưng hoạt động" ? "Sẵn sàng" : "Ngưng hoạt động";
            var (thanhCong, thongBao) = _vehicleService.ChuyenTrangThaiNhanh(XeChiTiet.MaXe, trangThaiMoi);
            if (!thanhCong)
            {
                MessageBox.Show(thongBao, "Cảnh báo nghiệp vụ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DangMoChiTiet = false;
            ThongBaoHeThong = thongBao;
            LoadDuLieu();
        }

        private void ThucHienChonAnh()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn hình ảnh xe",
                Filter = "Tệp hình ảnh (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                string thuMucAnh = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VehicleImages");
                Directory.CreateDirectory(thuMucAnh);

                string tenFile = $"{Guid.NewGuid():N}{Path.GetExtension(dialog.FileName)}";
                string duongDanDich = Path.Combine(thuMucAnh, tenFile);
                File.Copy(dialog.FileName, duongDanDich, true);

                FormHinhAnh = Path.Combine("VehicleImages", tenFile);
            }
            catch (Exception ex)
            {
                ThongBaoLoiForm = $"Không thể tải ảnh: {ex.Message}";
            }
        }

        private static string ChuanHoaSo(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return input.Trim()
                .Replace(".", "")
                .Replace(",", "")
                .Replace(" ", "")
                .Replace("₫", "")
                .Replace("đ", "")
                .Replace("VNĐ", "", StringComparison.OrdinalIgnoreCase);
        }
    }
}
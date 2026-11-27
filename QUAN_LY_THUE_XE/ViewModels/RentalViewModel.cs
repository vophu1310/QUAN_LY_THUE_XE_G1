using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QUAN_LY_THUE_XE.Data;
using QUAN_LY_THUE_XE.Helpers;
using QUAN_LY_THUE_XE.Models;
using QUAN_LY_THUE_XE.Services;

namespace QUAN_LY_THUE_XE.ViewModels
{
    public class RentalViewModel : BaseViewModel
    {
        private readonly RentalService _rentalService;

        // 1. Quản lý Tab hiển thị (0: Lập hợp đồng mới, 1: Danh sách hợp đồng)
        private int _tabDangChon = 0;
        public int TabDangChon
        {
            get => _tabDangChon;
            set { _tabDangChon = value; OnPropertyChanged(); }
        }

        public ICommand ChonTabCommand { get; }

        // 2. Danh sách Khách, Xe và Hợp đồng
        public ObservableCollection<KhachHang> DanhSachKhachHang { get; set; } = new();
        public ObservableCollection<Xe> DanhSachXe { get; set; } = new();
        public ObservableCollection<DonThue> DanhSachDonThue { get; set; } = new();

        // 3. Đối tượng được chọn
        private KhachHang? _khachHangDuocChon;
        public KhachHang? KhachHangDuocChon
        {
            get => _khachHangDuocChon;
            set
            {
                _khachHangDuocChon = value;
                OnPropertyChanged();
                ThongBaoLoi = "";
            }
        }

        private Xe? _xeDuocChon;
        public Xe? XeDuocChon
        {
            get => _xeDuocChon;
            set
            {
                _xeDuocChon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CoXeSanSang));
                OnPropertyChanged(nameof(HetXe));
                OnPropertyChanged(nameof(ChiTietDonGiaThue));
                OnPropertyChanged(nameof(HinhAnhHienThi));
                TinhToanChiPhi();
                ThongBaoLoi = "";
            }
        }

        // Thuộc tính cờ trạng thái: Còn xe sẵn sàng và Hết xe
        public bool CoXeSanSang => DanhSachXe.Count > 0 && XeDuocChon != null;
        public bool HetXe => DanhSachXe.Count == 0;

        // Chi tiết công thức tính tiền thuê hiển thị trên hóa đơn
        public string ChiTietDonGiaThue => XeDuocChon != null
            ? $"{SoNgayThue} ngày x {XeDuocChon.GiaThueNgay:N0} đ"
            : "0 ngày x 0 đ";

        // Ảnh đại diện xe hiển thị theo xe đang chọn (mặc định là Mazda CX-5 nếu chưa có ảnh)
        public string HinhAnhHienThi => !string.IsNullOrEmpty(XeDuocChon?.HinhAnh)
            ? XeDuocChon.HinhAnh
            : "https://i.postimg.cc/QdT44Lxc/2022-mazda-cx-5-2p5-turbo-signature-123-1657559083.avif";

        // 4. Ngày thuê và ngày trả
        private DateTime _ngayBatDau = DateTime.Today;
        public DateTime NgayBatDau
        {
            get => _ngayBatDau;
            set
            {
                _ngayBatDau = value;
                OnPropertyChanged();
                TinhToanChiPhi();
                OnPropertyChanged(nameof(ChiTietDonGiaThue));
            }
        }

        private DateTime _ngayKetThuc = DateTime.Today.AddDays(5);
        public DateTime NgayKetThuc
        {
            get => _ngayKetThuc;
            set
            {
                _ngayKetThuc = value;
                OnPropertyChanged();
                TinhToanChiPhi();
                OnPropertyChanged(nameof(ChiTietDonGiaThue));
            }
        }

        // 5. Các con số chi phí
        private int _soNgayThue = 5;
        public int SoNgayThue
        {
            get => _soNgayThue;
            set { _soNgayThue = value; OnPropertyChanged(); }
        }

        private decimal _tongTienThue = 0;
        public decimal TongTienThue
        {
            get => _tongTienThue;
            set { _tongTienThue = value; OnPropertyChanged(); }
        }

        private decimal _thueVAT = 0;
        public decimal ThueVAT
        {
            get => _thueVAT;
            set { _thueVAT = value; OnPropertyChanged(); }
        }

        private decimal _tongCong = 0;
        public decimal TongCong
        {
            get => _tongCong;
            set { _tongCong = value; OnPropertyChanged(); }
        }

        private decimal _tienCoc = 0;
        public decimal TienCoc
        {
            get => _tienCoc;
            set
            {
                _tienCoc = value;
                OnPropertyChanged();
                TienConLai = TongCong - _tienCoc;
            }
        }

        private decimal _tienConLai = 0;
        public decimal TienConLai
        {
            get => _tienConLai;
            set { _tienConLai = value; OnPropertyChanged(); }
        }

        // 6. Thông báo
        private string _thongBao = "";
        public string ThongBao
        {
            get => _thongBao;
            set { _thongBao = value; OnPropertyChanged(); }
        }

        private string _thongBaoLoi = "";
        public string ThongBaoLoi
        {
            get => _thongBaoLoi;
            set { _thongBaoLoi = value; OnPropertyChanged(); }
        }

        // 7. Lệnh thao tác
        public ICommand TaoDonThueCommand { get; }
        public ICommand TraXeCommand { get; }
        public ICommand LamMoiCommand { get; }

        public RentalViewModel()
        {
            var context = new AppDbContext();
            _rentalService = new RentalService(context);

            ChonTabCommand = new RelayCommand(p =>
            {
                if (p != null && int.TryParse(p.ToString(), out int tabIndex))
                    TabDangChon = tabIndex;
            });

            TaoDonThueCommand = new RelayCommand(ThucHienTaoDonThue, _ => CoXeSanSang);
            TraXeCommand = new RelayCommand(ThucHienTraXe);
            LamMoiCommand = new RelayCommand(_ => LoadDuLieu());
            ThemKhachHangCommand = new RelayCommand(_ => ThemKhachHangMoi());

            LoadDuLieu();
        }

        public ICommand ThemKhachHangCommand { get; }

        private void ThemKhachHangMoi()
        {
            try
            {
                var customerService = new CustomerService();
                int maKHMoi = customerService.LayMaKHKeTiep();
                var khachMoi = new KhachHang
                {
                    MaKH = maKHMoi,
                    HoTen = string.Empty,
                    SODT = string.Empty,
                    CCCD = string.Empty,
                    GPLX = string.Empty,
                    TrangThaiXacMinh = "Chưa gửi"
                };

                var dialog = new Views.AddCustomerDialog(khachMoi);
                if (dialog.ShowDialog() == true)
                {
                    int rows = customerService.ThemKhachHang(dialog.KhachHangMoi);
                    if (rows > 0)
                    {
                        LoadDuLieu();
                        KhachHangDuocChon = DanhSachKhachHang.FirstOrDefault(k => k.MaKH == dialog.KhachHangMoi.MaKH)
                                            ?? DanhSachKhachHang.LastOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadDuLieu()
        {
            using var context = new AppDbContext();

            // Khởi tạo dữ liệu xe mẫu nếu CSDL chưa có xe nào
            if (!context.Xes.Any())
            {
                context.Xes.AddRange(
                    new Xe
                    {
                        BienSo = "51K-888.88",
                        TenXe = "Mazda CX-5 2.0 Premium",
                        HangXe = "Mazda",
                        NamSanXuat = 2023,
                        SoChoNgoi = 5,
                        GiaThueNgay = 1000000,
                        SoKmHienTai = 8000,
                        TrangThai = "Sẵn sàng",
                        HinhAnh = "https://i.postimg.cc/QdT44Lxc/2022-mazda-cx-5-2p5-turbo-signature-123-1657559083.avif"
                    },
                    new Xe
                    {
                        BienSo = "51F-123.45",
                        TenXe = "Toyota Vios 1.5G",
                        HangXe = "Toyota",
                        NamSanXuat = 2022,
                        SoChoNgoi = 5,
                        GiaThueNgay = 700000,
                        SoKmHienTai = 15000,
                        TrangThai = "Sẵn sàng",
                        HinhAnh = "https://i.postimg.cc/HW72tK0P/images.jpg"
                    }
                );
                context.SaveChanges();
            }
            else
            {
                // Cập nhật ảnh cho Toyota Vios nếu trong CSDL đã có bản ghi
                var vios = context.Xes.FirstOrDefault(x => x.TenXe.Contains("Vios") || x.BienSo == "51F-123.45");
                if (vios != null && vios.HinhAnh != "https://i.postimg.cc/HW72tK0P/images.jpg")
                {
                    vios.HinhAnh = "https://i.postimg.cc/HW72tK0P/images.jpg";
                    context.SaveChanges();
                }

                // Cập nhật ảnh cho Mazda CX-5 
                var mazda = context.Xes.FirstOrDefault(x => x.HangXe == "Mazda" || x.TenXe.Contains("CX-5"));
                if (mazda != null && string.IsNullOrEmpty(mazda.HinhAnh))
                {
                    mazda.HinhAnh = "https://i.postimg.cc/QdT44Lxc/2022-mazda-cx-5-2p5-turbo-signature-123-1657559083.avif";
                    context.SaveChanges();
                }
            }

            // Đồng bộ và nạp danh sách Khách hàng từ phân hệ Khách hàng của bạn Hoàng
            var customerService = new CustomerService();
            var dsKhachHoang = customerService.LayTatCaKhachHang();

            // Đảm bảo khách hàng của Hoàng tồn tại trong DbContext để phục vụ tạo Hợp đồng (khóa ngoại MaKH)
            int maTKMacDinh = context.TaiKhoans.Select(t => t.MaTK).FirstOrDefault();
            foreach (var kh in dsKhachHoang)
            {
                var existing = context.KhachHangs.Find(kh.MaKH);
                if (existing == null)
                {
                    kh.MaTK = maTKMacDinh;
                    context.KhachHangs.Add(kh);
                }
                else
                {
                    existing.HoTen = kh.HoTen;
                    existing.SDT = kh.SDT;
                    existing.CCCD = kh.CCCD;
                    existing.GPLX = kh.GPLX;
                    existing.IsBlacklisted = kh.IsBlacklisted;
                }
            }

            // Dọn dẹp khách hàng mẫu cũ nếu không gắn với hợp đồng nào
            var idsHoang = dsKhachHoang.Select(k => k.MaKH).ToHashSet();
            var khachMauCu = context.KhachHangs
                .Where(k => !idsHoang.Contains(k.MaKH) && !context.DonThues.Any(d => d.MaKH == k.MaKH))
                .ToList();
            if (khachMauCu.Count > 0)
            {
                context.KhachHangs.RemoveRange(khachMauCu);
            }
            context.SaveChanges();

            // Nạp danh sách Khách hàng (lấy từ dữ liệu của bạn Hoàng)
            DanhSachKhachHang.Clear();
            foreach (var kh in dsKhachHoang)
                DanhSachKhachHang.Add(kh);

            // Nạp danh sách Xe đang sẵn sàng
            DanhSachXe.Clear();
            foreach (var xe in _rentalService.LayDanhSachXeSanSang())
                DanhSachXe.Add(xe);

            // Nạp danh sách Hợp đồng
            DanhSachDonThue.Clear();
            foreach (var dt in _rentalService.LayDanhSachDonThue())
                DanhSachDonThue.Add(dt);

            if (KhachHangDuocChon == null && DanhSachKhachHang.Count > 0)
                KhachHangDuocChon = DanhSachKhachHang[0];

            // LOGIC XỬ LÝ LỰA CHỌN XE & HẾT XE
            if (DanhSachXe.Count > 0)
            {
                // Nếu xe đang chọn vẫn còn trong danh sách xe sẵn sàng thì giữ nguyên
                if (XeDuocChon == null || !DanhSachXe.Any(x => x.MaXe == XeDuocChon.MaXe))
                {
                    XeDuocChon = DanhSachXe.FirstOrDefault(x =>
                        x.HangXe == "Mazda" || x.TenXe.Contains("CX-5"))
                        ?? DanhSachXe[0];
                }
                ThongBaoLoi = "";
            }
            else
            {
                //Khi hết xe sẵn sàng, reset XeDuocChon về null
                XeDuocChon = null;
                ThongBaoLoi = "⚠️ Hiện tại tất cả các xe đều đang được thuê hoặc đang bảo dưỡng. Vui lòng nhận xe trả ở tab 'Danh sách hợp đồng' hoặc quay lại sau!";
            }

            OnPropertyChanged(nameof(CoXeSanSang));
            OnPropertyChanged(nameof(HetXe));
            OnPropertyChanged(nameof(ChiTietDonGiaThue));
            OnPropertyChanged(nameof(HinhAnhHienThi));

            TinhToanChiPhi();
        }
        // Tính toán chi phí thuê xe dựa trên ngày thuê, ngày trả và giá thuê của xe
        private void TinhToanChiPhi()
        {
            int soNgay = (int)(NgayKetThuc.Date - NgayBatDau.Date).TotalDays;
            SoNgayThue = soNgay > 0 ? soNgay : 1;

            if (XeDuocChon != null)
            {
                TongTienThue = SoNgayThue * XeDuocChon.GiaThueNgay;
                ThueVAT = TongTienThue * 0.1m;
                TongCong = TongTienThue + ThueVAT;
                TienCoc = TongCong * 0.3m;
                TienConLai = TongCong - TienCoc;
            }
            else
            {
                TongTienThue = 0;
                ThueVAT = 0;
                TongCong = 0;
                TienCoc = 0;
                TienConLai = 0;
            }
        }

        private void ThucHienTaoDonThue(object? parameter)
        {
            ThongBaoLoi = "";
            ThongBao = "";

            if (KhachHangDuocChon == null)
            {
                ThongBaoLoi = "Vui lòng chọn khách hàng thuê xe!";
                return;
            }

            if (HetXe || XeDuocChon == null)
            {
                ThongBaoLoi = "Hiện tại không còn xe nào sẵn sàng để tạo hợp đồng thuê!";
                return;
            }

            // Kiểm tra kép xem xe có còn đang sẵn sàng không (tránh trường hợp xe vừa được thuê bởi phiên khác)
            if (!DanhSachXe.Any(x => x.MaXe == XeDuocChon.MaXe))
            {
                ThongBaoLoi = $"Xe '{XeDuocChon.TenXe}' hiện không còn sẵn sàng (vừa có khách thuê hoặc bảo dưỡng). Vui lòng chọn xe khác!";
                LoadDuLieu();
                return;
            }

            if (NgayKetThuc <= NgayBatDau)
            {
                ThongBaoLoi = "Ngày trả xe phải sau ngày nhận xe ít nhất 1 ngày!";
                return;
            }

            bool thanhCong = _rentalService.TaoDonThue(
                KhachHangDuocChon.MaKH,
                XeDuocChon.MaXe,
                NgayBatDau,
                NgayKetThuc,
                TongCong,
                TienCoc
            );

            if (thanhCong)
            {
                System.Windows.MessageBox.Show("🎉 Tạo hợp đồng thuê xe thành công!\nHệ thống sẽ chuyển bạn sang Danh sách hợp đồng để theo dõi.",
                    "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                LoadDuLieu();
                TabDangChon = 1; // TỰ ĐỘNG CHUYỂN SANG TAB DANH SÁCH HỢP ĐỒNG ĐỂ NHÂN VIÊN THẤY
            }
            else
            {
                ThongBaoLoi = "Lỗi khi lưu hợp đồng vào cơ sở dữ liệu! Xe có thể vừa được người khác thuê trước.";
                LoadDuLieu();
            }
        }

        // Xác nhận trả xe

        private void ThucHienTraXe(object? parameter)
        {
            if (parameter is DonThue donThue)
            {
                var xacNhan = System.Windows.MessageBox.Show(
                    $"Bạn có chắc chắn muốn xác nhận trả xe cho Hợp đồng #{donThue.MaDon}?\n" +
                    $"• Khách: {donThue.KhachHang?.HoTen}\n" +
                    $"• Xe: {donThue.Xe?.TenXe} ({donThue.Xe?.BienSo})\n" +
                    $"• Số tiền thu nốt lúc trả xe: {(donThue.TongTienDuKien - donThue.TienCoc):N0} VNĐ",
                    "Xác nhận trả xe",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (xacNhan == System.Windows.MessageBoxResult.Yes)
                {
                    bool thanhCong = _rentalService.XacNhanTraXe(donThue.MaDon);
                    if (thanhCong)
                    {
                        System.Windows.MessageBox.Show("✅ Đã hoàn tất thủ tục trả xe!\nChiếc xe đã quay lại trạng thái 'Sẵn sàng' cho thuê.",
                            "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        LoadDuLieu();
                    }
                }
            }
        }
    }
}
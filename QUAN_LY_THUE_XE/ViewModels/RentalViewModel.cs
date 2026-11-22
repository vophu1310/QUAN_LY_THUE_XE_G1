using System;
using System.Collections.ObjectModel;
using System.Linq;
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
                TinhToanChiPhi();
            }
        }

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

            TaoDonThueCommand = new RelayCommand(ThucHienTaoDonThue);
            TraXeCommand = new RelayCommand(ThucHienTraXe);
            LamMoiCommand = new RelayCommand(_ => LoadDuLieu());

            LoadDuLieu();
        }

        public void LoadDuLieu()
        {
            using var context = new AppDbContext();

            // Cập nhật xe Mazda CX-5 làm xe mặc định nếu xe này đã có trong CSDL.
            var xeMacDinh = context.Xes.FirstOrDefault(x =>
                x.HangXe == "Mazda" || x.TenXe.Contains("CX-5"));

            // Nếu CSDL chưa có xe Mazda thì dùng bản ghi đầu tiên để tạo dữ liệu mẫu.
            xeMacDinh ??= context.Xes.FirstOrDefault();
            if (xeMacDinh != null)
            {
                xeMacDinh.TenXe = "Mazda CX-5 2.0 Premium";
                xeMacDinh.BienSo = "51K-888.88";
                xeMacDinh.HangXe = "Mazda";
                xeMacDinh.GiaThueNgay = 1000000; // Giá thuê 1.000.000 đ / ngày
                context.SaveChanges();
            }

            if (!context.KhachHangs.Any())
            {
                int maTKMacDinh = context.TaiKhoans.Select(t => t.MaTK).FirstOrDefault();
                context.KhachHangs.AddRange(
                    new KhachHang
                    {
                        MaTK = maTKMacDinh,
                        HoTen = "Phạm Nhật Vượng",
                        SDT = "0901234567",
                        CCCD = "001089012345",
                        GPLX = "B2-998877",
                        TrangThaiXacMinh = "Đã xác minh"
                    },
                    new KhachHang
                    {
                        MaTK = maTKMacDinh,
                        HoTen = "Trần Đình Long",
                        SDT = "0988665544",
                        CCCD = "079090123456",
                        GPLX = "B2-554433",
                        TrangThaiXacMinh = "Đã xác minh"
                    }
                );
                context.SaveChanges();
            }

            // Nạp danh sách
            DanhSachKhachHang.Clear();
            foreach (var kh in _rentalService.LayDanhSachKhachHang())
                DanhSachKhachHang.Add(kh);

            DanhSachXe.Clear();
            foreach (var xe in _rentalService.LayDanhSachXeSanSang())
                DanhSachXe.Add(xe);

            DanhSachDonThue.Clear();
            foreach (var dt in _rentalService.LayDanhSachDonThue())
                DanhSachDonThue.Add(dt);

            if (KhachHangDuocChon == null && DanhSachKhachHang.Count > 0)
                KhachHangDuocChon = DanhSachKhachHang[0];

            if (DanhSachXe.Count > 0)
            {
                XeDuocChon = DanhSachXe.FirstOrDefault(x =>
                    x.HangXe == "Mazda" || x.TenXe.Contains("CX-5"))
                    ?? DanhSachXe[0];
            }

            TinhToanChiPhi();
        }

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

            if (XeDuocChon == null)
            {
                ThongBaoLoi = "Vui lòng chọn xe cần thuê!";
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
                TabDangChon = 1; // TỰ ĐỘNG CHUYỂN SANG TAB DANH SÁCH HỢP ĐỒNG ĐỂ NHÂN VIÊN THẤY NGAY!
            }
            else
            {
                ThongBaoLoi = "Lỗi khi lưu hợp đồng vào cơ sở dữ liệu!";
            }
        }

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
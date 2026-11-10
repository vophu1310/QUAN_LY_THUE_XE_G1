# 🚗 HỆ THỐNG QUẢN LÝ CHO THUÊ XE (AUTO RENTAL)

> **Đồ án môn học:** Lập trình Trực quan (UIT - ĐHQG TP.HCM)  
> **Công nghệ:** C# WPF (.NET) • MVVM Pattern • Entity Framework Core • SQL Server

---

## 👥 Thành viên thực hiện (Nhóm 3 thành viên)

| STT | Họ và Tên | MSSV | Vai trò | Nhánh Git phụ trách |
| :---: | :--- | :--- | :--- | :--- |
| 1 | **Võ Nguyễn Thế Phú** | `25521415` | Kiến trúc hệ thống, Đăng nhập phân quyền, Hợp đồng Thuê/Trả & Giao nhận xe | `feature/rental-auth` |
| 2 | `Nguyễn Ngọc Vinh Huy` | `25520712` | Quản lý Đội xe, Lịch sử bảo dưỡng, Hóa đơn & Thanh toán VietQR | `feature/vehicle` |
| 3 | `Trần Minh Hoàng  ` | `25520606` | Quản lý Hồ sơ Khách hàng, Dashboard Thống kê doanh thu & Xuất báo cáo | `feature/customer-dashboard` |

---

## 🛠️ Công nghệ & Thư viện sử dụng

- **Nền tảng:** .NET 10.0 Windows (WPF - Windows Presentation Foundation)
- **Mô hình kiến trúc:** MVVM (Model - View - ViewModel) kết hợp Service Layer
- **Cơ sở dữ liệu:** SQL Server (LocalDB) & Entity Framework Core (Code-First)
- **Giao diện (UI/UX):** Material Design In XAML Toolkit (`MaterialDesignThemes`, `MaterialDesignColors`)
- **Biểu đồ thống kê:** `LiveChartsCore.SkiaSharpView.WPF`
- **Quản lý mã nguồn:** Git & GitHub (Feature Branch Workflow)

---

## 📱 Các màn hình chính của hệ thống (7 Screens)

1. **`LoginWindow`:** Đăng nhập và phân quyền theo vai trò (`Admin` / `Staff` / `Customer`).
2. **`DashboardView`:** Tổng quan doanh thu, số lượng xe rảnh/đang thuê, cảnh báo hạn đăng kiểm/bảo dưỡng & biểu đồ thống kê.
3. **`VehicleView`:** Quản lý danh mục đội xe dạng thẻ (Card View), bộ lọc đa tiêu chí, thêm/sửa/xóa và theo dõi bảo dưỡng.
4. **`RentalView`:** Quản lý đặt xe và hợp đồng thuê (kiểm tra chống trùng lịch, tính tiền cọc).
5. **`HandoverView`:** Lập biên bản bàn giao và nhận xe (Check-in/Check-out: số Km Odo, mức nhiên liệu, kiểm tra trầy xước).
6. **`CustomerView`:** Quản lý hồ sơ khách hàng (CCCD, SĐT, GPLX), lịch sử thuê xe và danh sách đen (Blacklist).
7. **`FinanceView`:** Quản lý thu chi, công nợ, tính toán phụ phí phát sinh, thanh toán hóa đơn và tạo mã **VietQR**.

---

## 📜 Quy tắc nghiệp vụ cốt lõi (Business Rules)

- **BR-01 (Toàn vẹn trạng thái xe):** Không cho phép xóa xe đang có trạng thái *'Đang được đặt'* hoặc *'Đang cho thuê'*. Chỉ được chuyển sang *'Ngưng hoạt động'* khi xe đang ở bãi.
- **BR-02 (Khóa lịch tức thời):** Ngay khi ghi nhận đặt cọc thành công, hệ thống tự động khóa lịch xe và chuyển trạng thái sang *'Đã đặt'* để tránh trùng lịch.
- **BR-03 (Chính sách hủy đơn & hoàn cọc):** Khi hủy đơn hợp lệ, hệ thống tự động hoàn trả **70%** tiền cọc, giữ lại **30%** làm phí phạt và mở lại trạng thái xe.
- **BR-04 (Công thức quyết toán trả xe):**  
  `Số tiền thanh toán cuối cùng = Tiền thuê thực tế + Chi phí phát sinh - Tiền đặt cọc ban đầu`
- **BR-05 (Điều kiện nhận xe):** Khách hàng bắt buộc phải cung cấp đầy đủ **SĐT, CCCD và GPLX** hợp lệ trước khi đơn đặt được nhân viên duyệt.

---

## 📂 Cấu trúc thư mục dự án

```text
QUAN_LY_THUE_XE/
├── Data/           # AppDbContext kết nối SQL Server & DbInitializer tạo dữ liệu mẫu
├── Helpers/        # RelayCommand, CurrencyConverter, StatusToColorConverter
├── Models/         # 9 bảng dữ liệu (Xe, LoaiXe, KhachHang, DonThue, HoaDon, BienBanGiaoNhan...)
├── Resources/      # Phông chữ (Noto, Roboto) và tài nguyên hình ảnh
├── Services/       # Tầng xử lý logic nghiệp vụ và truy vấn CSDL
├── ViewModels/     # BaseViewModel và các lớp điều phối dữ liệu cho Views
├── Views/          # Toàn bộ 7 màn hình giao diện XAML
└── MainWindow.xaml # Cửa sổ điều hướng chính (Sidebar Menu)
```

---

## 🚀 Hướng dẫn cài đặt & Chạy thử (Quick Start)

1. **Clone dự án về máy:**
   ```bash
   git clone https://github.com/vophu1310/QUAN_LY_THUE_XE_G1.git
   ```
2. Mở file **`QUAN_LY_THUE_XE.slnx`** bằng **Visual Studio**.
3. Chuyển sang nhánh làm việc cá nhân trước khi code.
4. Bấm **`F5`** để chạy ứng dụng.  
   *(Ứng dụng sẽ tự động khởi tạo cơ sở dữ liệu `QuanLyThueXeDB` trên `(localdb)\mssqllocaldb` và nạp sẵn tài khoản quản trị mặc định `admin` / `123456` cùng danh sách xe mẫu).*

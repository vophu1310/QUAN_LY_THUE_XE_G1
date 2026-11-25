using Microsoft.EntityFrameworkCore;
using QUAN_LY_THUE_XE.Models;

namespace QUAN_LY_THUE_XE.Data
{
    public class AppDbContext : DbContext
    {
        // Khai báo các bảng trong CSDL
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<Xe> Xes { get; set; }
        public DbSet<DonThue> DonThues { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<PhuPhi> PhuPhis { get; set; }
        public DbSet<BienBanGiaoNhan> BienBanGiaoNhans { get; set; }

        // Nếu bạn đã tạo thêm 2 Model này thì bỏ comment:
        // public DbSet<LoaiXe> LoaiXes { get; set; }
        // public DbSet<ChiTietPhuPhi> ChiTietPhuPhis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Chuỗi kết nối tới SQL Server Local (hoặc thay bằng tên server SQL trên máy bạn)
                // (localdb)\mssqllocaldb là server mặc định có sẵn khi cài Visual Studio
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=QuanLyThueXeDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình kiểu dữ liệu tiền tệ để không bị cảnh báo độ chính xác trong EF Core
            modelBuilder.Entity<Xe>().Property(x => x.GiaThueNgay).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DonThue>().Property(d => d.TienCoc).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DonThue>().Property(d => d.TongTienDuKien).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<HoaDon>().Property(h => h.TienThue).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<HoaDon>().Property(h => h.TongPhuPhi).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<HoaDon>().Property(h => h.SoTienThanhToan).HasColumnType("decimal(18,2)");
        }
    }
}
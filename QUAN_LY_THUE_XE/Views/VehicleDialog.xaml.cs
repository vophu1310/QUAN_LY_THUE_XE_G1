using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using QUAN_LY_THUE_XE.Models;
using QUAN_LY_THUE_XE.Services;

namespace QUAN_LY_THUE_XE.Views
{
    public enum VehicleDialogMode
    {
        Add,
        Edit,
        Detail
    }

    public partial class VehicleDialog : Window
    {
        private readonly VehicleDialogMode _mode;
        private readonly Xe? _xeGoc;

        public Xe? KetQuaXe { get; private set; }
        public bool YeuCauChuyenSangSua { get; private set; }

        public VehicleDialog(Xe? xe, VehicleDialogMode mode)
        {
            InitializeComponent();
            _xeGoc = xe;
            _mode = mode;

            KhoiTaoGiaoDien();
        }

        private void KhoiTaoGiaoDien()
        {
            if (_mode == VehicleDialogMode.Add)
            {
                Title = "Thêm xe mới";
                txtTieuDe.Text = "Thêm xe mới";
                txtMoTa.Text = "Nhập đầy đủ thông số kỹ thuật và giá thuê để đưa xe vào hệ thống";
                btnLuu.Content = "✓ Lưu xe mới";

                tbNamSanXuat.Text = DateTime.Now.Year.ToString();
                tbSoChoNgoi.Text = "5";
                tbSoKm.Text = "0";
                tbTienCoc.Text = "0";
                cbTrangThai.SelectedIndex = 0; // Sẵn sàng
                return;
            }

            if (_xeGoc == null) return;

            // Đổ dữ liệu xe hiện có vào form
            tbBienSo.Text = _xeGoc.BienSo;
            tbTenXe.Text = _xeGoc.TenXe;
            cbHangXe.Text = _xeGoc.HangXe;
            tbNamSanXuat.Text = _xeGoc.NamSanXuat.ToString();
            tbSoChoNgoi.Text = _xeGoc.SoChoNgoi.ToString();
            tbSoKm.Text = _xeGoc.SoKmHienTai.ToString();
            tbGiaThueNgay.Text = _xeGoc.GiaThueNgay.ToString("0", CultureInfo.InvariantCulture);
            tbTienCoc.Text = _xeGoc.TienCocMacDinh.ToString("0", CultureInfo.InvariantCulture);
            tbHinhAnh.Text = _xeGoc.HinhAnh ?? string.Empty;

            ChonTrangThaiTrongComboBox(_xeGoc.TrangThai);
            CapNhatAnhXemTruoc(_xeGoc.HinhAnh);

            if (_mode == VehicleDialogMode.Edit)
            {
                Title = $"Chỉnh sửa xe - {_xeGoc.BienSo}";
                txtTieuDe.Text = "Chỉnh sửa thông tin xe";
                txtMoTa.Text = $"Cập nhật thông số cho xe {_xeGoc.TenXe} ({_xeGoc.BienSo})";
                btnLuu.Content = "✓ Lưu thay đổi";

                // BR-01: Nếu xe đang được thuê hoặc đã đặt thì khóa thay đổi trạng thái
                if (VehicleService.LaTrangThaiDangBan(_xeGoc.TrangThai))
                {
                    cbTrangThai.IsEnabled = false;
                    txtTrangThaiNote.Visibility = Visibility.Visible;
                }
            }
            else if (_mode == VehicleDialogMode.Detail)
            {
                Title = $"Chi tiết xe - {_xeGoc.BienSo}";
                txtTieuDe.Text = _xeGoc.TenXe;
                txtMoTa.Text = $"Mã xe: XE{_xeGoc.MaXe:D3} · Biển số: {_xeGoc.BienSo} · Trạng thái: {_xeGoc.TrangThai}";

                tbBienSo.IsReadOnly = true;
                tbTenXe.IsReadOnly = true;
                cbHangXe.IsEnabled = false;
                tbNamSanXuat.IsReadOnly = true;
                tbSoChoNgoi.IsReadOnly = true;
                tbSoKm.IsReadOnly = true;
                tbGiaThueNgay.IsReadOnly = true;
                tbTienCoc.IsReadOnly = true;
                cbTrangThai.IsEnabled = false;
                tbHinhAnh.IsReadOnly = true;
                btnChonAnh.Visibility = Visibility.Collapsed;

                btnHuy.Content = "Đóng";
                btnLuu.Content = "✎ Chỉnh sửa xe";
            }
        }

        private void ChonTrangThaiTrongComboBox(string? trangThai)
        {
            if (string.IsNullOrWhiteSpace(trangThai))
            {
                cbTrangThai.SelectedIndex = 0;
                return;
            }

            foreach (var item in cbTrangThai.Items)
            {
                if (item is ComboBoxItem cbi && string.Equals(cbi.Content?.ToString(), trangThai, StringComparison.OrdinalIgnoreCase))
                {
                    cbTrangThai.SelectedItem = cbi;
                    return;
                }
            }

            // Nếu trạng thái là "Đang thuê" / "Đã đặt", thêm tạm mục hiển thị để người dùng thấy rõ
            var tempItem = new ComboBoxItem { Content = trangThai };
            cbTrangThai.Items.Add(tempItem);
            cbTrangThai.SelectedItem = tempItem;
        }

        private void BtnChonAnh_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Chọn hình ảnh phương tiện",
                Filter = "Ảnh (*.jpg;*.jpeg;*.png;*.bmp;*.webp)|*.jpg;*.jpeg;*.png;*.bmp;*.webp|Tất cả file (*.*)|*.*"
            };

            if (openDialog.ShowDialog() == true)
            {
                tbHinhAnh.Text = openDialog.FileName;
                CapNhatAnhXemTruoc(openDialog.FileName);
            }
        }

        private void TbHinhAnh_TextChanged(object sender, TextChangedEventArgs e)
        {
            CapNhatAnhXemTruoc(tbHinhAnh.Text);
        }

        private void CapNhatAnhXemTruoc(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                imgPreview.Source = null;
                pnlImagePlaceholder.Visibility = Visibility.Visible;
                return;
            }

            path = path.Trim();
            try
            {
                if (Uri.TryCreate(path, UriKind.Absolute, out Uri? uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgPreview.Source = bitmap;
                    pnlImagePlaceholder.Visibility = Visibility.Collapsed;
                    return;
                }

                if (File.Exists(path))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    imgPreview.Source = bitmap;
                    pnlImagePlaceholder.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            catch
            {
                // Bỏ qua nếu ảnh không đọc được
            }

            imgPreview.Source = null;
            pnlImagePlaceholder.Visibility = Visibility.Visible;
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (_mode == VehicleDialogMode.Detail)
            {
                YeuCauChuyenSangSua = true;
                DialogResult = false;
                Close();
                return;
            }

            string bienSo = tbBienSo.Text.Trim().ToUpperInvariant();
            string tenXe = tbTenXe.Text.Trim();
            string hangXe = cbHangXe.Text.Trim();

            if (string.IsNullOrWhiteSpace(bienSo))
            {
                MessageBox.Show("Vui lòng nhập biển số xe.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbBienSo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(tenXe))
            {
                MessageBox.Show("Vui lòng nhập tên xe.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbTenXe.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(hangXe))
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập hãng xe.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbHangXe.Focus();
                return;
            }

            int maxYear = DateTime.Now.Year + 1;
            if (!int.TryParse(tbNamSanXuat.Text.Trim(), out int namSanXuat) || namSanXuat < 1990 || namSanXuat > maxYear)
            {
                MessageBox.Show($"Năm sản xuất phải là số nguyên từ 1990 đến {maxYear}.", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbNamSanXuat.Focus();
                return;
            }

            if (!int.TryParse(tbSoChoNgoi.Text.Trim(), out int soChoNgoi) || soChoNgoi < 2 || soChoNgoi > 50)
            {
                MessageBox.Show("Số chỗ ngồi phải từ 2 đến 50.", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbSoChoNgoi.Focus();
                return;
            }

            string rawGia = tbGiaThueNgay.Text.Trim().Replace(",", "").Replace(".", "");
            if (!decimal.TryParse(rawGia, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal giaThueNgay) || giaThueNgay <= 0)
            {
                MessageBox.Show("Giá thuê/ngày phải là số dương lớn hơn 0.", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbGiaThueNgay.Focus();
                return;
            }

            decimal tienCoc = 0;
            string rawCoc = tbTienCoc.Text.Trim().Replace(",", "").Replace(".", "");
            if (!string.IsNullOrEmpty(rawCoc))
            {
                if (!decimal.TryParse(rawCoc, NumberStyles.Number, CultureInfo.InvariantCulture, out tienCoc) || tienCoc < 0)
                {
                    MessageBox.Show("Tiền cọc mặc định không được âm.", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    tbTienCoc.Focus();
                    return;
                }
            }

            int soKm = 0;
            string rawKm = tbSoKm.Text.Trim().Replace(",", "").Replace(".", "");
            if (!string.IsNullOrEmpty(rawKm))
            {
                if (!int.TryParse(rawKm, out soKm) || soKm < 0)
                {
                    MessageBox.Show("Số Km (ODO) phải là số nguyên không âm.", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    tbSoKm.Focus();
                    return;
                }
            }

            string trangThai = (cbTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString()
                               ?? _xeGoc?.TrangThai
                               ?? VehicleService.TrangThaiSanSang;

            KetQuaXe = new Xe
            {
                MaXe = _xeGoc?.MaXe ?? 0,
                BienSo = bienSo,
                TenXe = tenXe,
                HangXe = hangXe,
                NamSanXuat = namSanXuat,
                SoChoNgoi = soChoNgoi,
                GiaThueNgay = giaThueNgay,
                TienCocMacDinh = tienCoc,
                SoKmHienTai = soKm,
                HinhAnh = tbHinhAnh.Text.Trim(),
                TrangThai = trangThai,
                MaLoaiXe = _xeGoc?.MaLoaiXe
            };

            DialogResult = true;
            Close();
        }
    }
}

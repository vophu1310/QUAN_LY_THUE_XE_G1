using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QUAN_LY_THUE_XE.ViewModels
{
    // Lớp nền để các ViewModel thông báo WPF khi thuộc tính bind thay đổi.
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Báo cho binding cập nhật thuộc tính có tên được truyền hoặc tên thuộc tính gọi hàm.
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Chỉ cập nhật và phát thông báo khi giá trị thật sự đổi; trả false nếu không đổi.
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

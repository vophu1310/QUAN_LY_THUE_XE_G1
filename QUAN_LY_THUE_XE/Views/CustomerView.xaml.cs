using QUAN_LY_THUE_XE.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace QUAN_LY_THUE_XE.Views
{
    /// <summary>
    /// Code-behind cho CustomerView.
    /// Theo kiến trúc MVVM, file này chỉ gắn DataContext.
    /// Toàn bộ logic nằm trong CustomerViewModel.
    /// </summary>
    public partial class CustomerView : UserControl
    {
        public CustomerView()
        {
            InitializeComponent();
            // Gắn ViewModel làm DataContext — XAML sẽ bind tất cả từ đây.
            DataContext = new CustomerViewModel();
        }

        // Nhấp đúp chuột vào bất kỳ dòng khách hàng nào để mở nhanh hộp thoại chỉnh sửa
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is CustomerViewModel vm)
            {
                vm.EditCommand.Execute(null);
            }
        }
    }
}

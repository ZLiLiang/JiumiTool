using CommunityToolkit.Mvvm.ComponentModel;

namespace JiumiTool2.ViewModels
{
    public partial class FileConfigDialogModel : ObservableObject
    {
        [ObservableProperty]
        private string _fileName = string.Empty;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JiumiTool2.Commons;
using Wpf.Ui;

namespace JiumiTool2.ViewModels
{
    public partial class JiumiViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        public JiumiViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private void OnCardClick(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return;
            }

            Type? pageType = NameToPageTypeConverter.Convert(parameter);

            if (pageType == null)
            {
                return;
            }

            _navigationService.Navigate(pageType);
        }
    }
}

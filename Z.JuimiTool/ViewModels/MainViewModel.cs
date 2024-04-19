using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Z.JiumiTool.Common;
using Z.JiumiTool.Constants;

namespace Z.JiumiTool.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {
        private readonly IRegionManager _regionManager;
        private string fileUrl = ImageUrl.FileBefore;
        private string videoUrl = ImageUrl.VideoBefore;

        public DelegateCommand FileCommand { get; private set; }
        public DelegateCommand VideoCommand { get; private set; }
        public DelegateCommand ImageCommand { get; private set; }

        public string FileUrl { get => fileUrl; set => SetProperty(ref fileUrl, value); }
        public string VideoUrl { get => videoUrl; set => SetProperty(ref videoUrl, value); }

        public MainViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            FileCommand = new DelegateCommand(FileExecute);
            VideoCommand = new DelegateCommand(VideoExecute);
            ImageCommand = new DelegateCommand(ImageExecute);
        }

        #region 私有执行

        private void FileExecute()
        {
            SetImagesBefore();
            FileUrl = ImageUrl.FileAfter;

            _regionManager.Regions["ContentRegion"].RequestNavigate("FileView");
        }


        private void VideoExecute()
        {
            SetImagesBefore();
            VideoUrl = ImageUrl.VideoAfter;

            _regionManager.Regions["ContentRegion"].RequestNavigate("VideoView");
        }


        private void ImageExecute()
        {
            SetImagesBefore();

            _regionManager.Regions["ContentRegion"].RequestNavigate("JuimiView");
        }

        #endregion

        private void SetImagesBefore()
        {
            FileUrl = ImageUrl.FileBefore;
            VideoUrl = ImageUrl.VideoBefore;
        }

        public void Configure()
        {
            _regionManager.Regions["ContentRegion"].RequestNavigate("JuimiView");
        }
    }
}

using DiskSpaceAnalyzer.Model;
using DiskSpaceAnalyzer.Mvvm;

namespace DiskSpaceAnalyzer.ViewModel
{
    public class MainViewModel : NotificationObject
    {
        private readonly MainModel _mainModel;

        public DisksViewModel DisksViewModel { get; }

        public DiskItemsViewModel DiskItemsViewModel { get; }

        public StatusBarViewModel StatusBarViewModel { get; }

        public MainViewModel()
        {
            _mainModel = new MainModel();
            DisksViewModel = new DisksViewModel(_mainModel);
            DiskItemsViewModel = new DiskItemsViewModel(_mainModel);
            StatusBarViewModel = new StatusBarViewModel(_mainModel);
        }
    }
}

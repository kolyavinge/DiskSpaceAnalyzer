using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DiskSpaceAnalyzer.Model;
using DiskSpaceAnalyzer.Mvvm;
using DiskSpaceAnalyzer.Tool;

namespace DiskSpaceAnalyzer.ViewModel
{
    public class DiskItemsViewModel : NotificationObject
    {
        private readonly MainModel _mainModel;

        private IEnumerable<DiskItemViewModel> _items;
        public IEnumerable<DiskItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged("Items");
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _isGotoUpCommandEnabled;
        public bool IsGotoUpCommandEnabled
        {
            get { return _isGotoUpCommandEnabled; }
            set
            {
                _isGotoUpCommandEnabled = value;
                RaisePropertyChanged("IsGotoUpCommandEnabled");
            }
        }

        private bool _isRefreshCommandEnabled;
        public bool IsRefreshCommandEnabled
        {
            get { return _isRefreshCommandEnabled; }
            set
            {
                _isRefreshCommandEnabled = value;
                RaisePropertyChanged("IsRefreshCommandEnabled");
            }
        }

        private string _historyFullPath;
        public string HistoryFullPath
        {
            get { return _historyFullPath; }
            set
            {
                _historyFullPath = value;
                RaisePropertyChanged("HistoryFullPath");
            }
        }

        public ICommand GotoDirectoryCommand { get { return new DelegateCommand<DiskItemViewModel>(GotoDirectory); } }

        public ICommand GotoUpCommand { get { return new DelegateCommand(GotoUp); } }

        public ICommand OpenCurrentDirectoryCommand { get { return new DelegateCommand(_mainModel.OpenCurrentDirectory); } }

        public ICommand RefreshCommand { get { return new DelegateCommand(_mainModel.Refresh); } }

        public DiskItemsViewModel(MainModel mainModel)
        {
            _mainModel = mainModel;
            _mainModel.OnAnalyzeDiskStart += OnAnalyzeDiskStart;
            _mainModel.OnAnalyzeDiskComplete += OnAnalyzeDiskComplete;
            IsEnabled = true;
            IsGotoUpCommandEnabled = false;
            IsRefreshCommandEnabled = false;
        }

        private void OnAnalyzeDiskStart(object sender, EventArgs e)
        {
            IsEnabled = false;
            Items = Enumerable.Empty<DiskItemViewModel>();
            UpdateHistoryFullPath();
            IsRefreshCommandEnabled = true;
        }

        private void OnAnalyzeDiskComplete(object sender, AnalyzeDiskCompleteEventArgs e)
        {
            UpdateHistoryFullPath();
            UpdateItems(e.Result.Root);
            IsEnabled = true;
            IsGotoUpCommandEnabled = _mainModel.IsGotoUpEnabled;
        }

        private void GotoDirectory(DiskItemViewModel selectedItem)
        {
            if (selectedItem.IsDirectory)
            {
                _mainModel.GotoDirectory(selectedItem.DiskItem);
                UpdateHistoryFullPath();
                UpdateItems(selectedItem.DiskItem);
                IsGotoUpCommandEnabled = _mainModel.IsGotoUpEnabled;
            }
        }

        private void GotoUp()
        {
            if (_mainModel.IsGotoUpEnabled)
            {
                var parent = _mainModel.GotoUp();
                UpdateHistoryFullPath();
                UpdateItems(parent);
                IsGotoUpCommandEnabled = _mainModel.IsGotoUpEnabled;
            }
        }

        private void UpdateItems(DiskItem diskItem)
        {
            Items = diskItem.Children.Select(item => new DiskItemViewModel(_mainModel.SelectedDist, item)).OrderByDescending(x => x.DiskItem.SizeBytes).ToList();
        }

        private void UpdateHistoryFullPath()
        {
            HistoryFullPath = _mainModel.HistoryFullPath;
        }
    }
}

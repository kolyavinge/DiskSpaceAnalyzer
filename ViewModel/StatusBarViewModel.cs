using System;
using DiskSpaceAnalyzer.Model;
using DiskSpaceAnalyzer.Mvvm;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace DiskSpaceAnalyzer.ViewModel
{
    public class StatusBarViewModel : NotificationObject
    {
        private float _progressValue;
        public float ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }

        private bool _inProgress;
        public bool IsProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                RaisePropertyChanged("IsProgress");
            }
        }

        public StatusBarViewModel(MainModel mainModel)
        {
            mainModel.OnAnalyzeDiskStart += OnAnalyzeDiskStart;
            mainModel.OnAnalyzeDiskProgress += OnAnalyzeDiskProgress;
            mainModel.OnAnalyzeDiskComplete += OnAnalyzeDiskComplete;
        }

        private void OnAnalyzeDiskStart(object sender, EventArgs e)
        {
            ProgressValue = 0;
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            IsProgress = true;
        }

        private void OnAnalyzeDiskProgress(object sender, AnalyzeProgressEventArgs e)
        {
            ProgressValue = e.ProgressPercent;
            TaskbarManager.Instance.SetProgressValue(e.ProgressPercent, 100);
        }

        private void OnAnalyzeDiskComplete(object sender, AnalyzeDiskCompleteEventArgs e)
        {
            ProgressValue = 0;
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            IsProgress = false;
        }
    }
}

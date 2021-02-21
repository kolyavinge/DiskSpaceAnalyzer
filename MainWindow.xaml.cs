using DiskSpaceAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiskSpaceAnalyzer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var diskProvider = new DiskProvider();
            var disks = diskProvider.GetDisks().ToList();
            var diskAnalyzer = new DiskAnalyzer();
            //diskAnalyzer.OnAnalyzeProgress += (s, e) =>
            //{
            //    Debug.WriteLine(e.ProgressPercent);
            //};
            var result = await diskAnalyzer.AnalyzeAsync(disks[0]);
        }
    }
}

using FxcCompilerGui.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FxcCompilerGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainVm = new MainWindowVM();
            var mainWnd = new MainWindow();

            EventHandler reqCloseHandler = null;
            reqCloseHandler = delegate
            {
                mainVm.RequestClose -= reqCloseHandler;
                mainWnd.Close();
            };

            mainVm.RequestClose += reqCloseHandler;

            EventHandler closedHandler = null;
            closedHandler = delegate
            {
                mainWnd.Closed -= closedHandler;
                mainVm.OnClosing();
            };

            mainWnd.Closed += closedHandler;

            mainWnd.DataContext = mainVm;
            mainWnd.Show();
        }
    }
}
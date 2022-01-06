using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ButiEngineEditor.Models;
using ButiEngineEditor.ViewModels;
using ButiEngineEditor.ViewModels.Documents;
using ButiEngineEditor.ViewModels.Panes;
using MahApps.Metro.Controls;

using Xceed.Wpf.AvalonDock.Layout.Serialization;
namespace ButiEngineEditor.Views
{
    /* 
     * If some events were receive from ViewModel, then please use PropertyChangedWeakEventListener and CollectionChangedWeakEventListener.
     * If you want to subscribe custome events, then you can use LivetWeakEventListener.
     * When window closing and any timing, Dispose method of LivetCompositeDisposable is useful to release subscribing events.
     *
     * Those events are managed using WeakEventListener, so it is not occurred memory leak, but you should release explicitly.
     */
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s,e)=> { CommunicateEachFrame.Start(); };
            Unloaded += (s, e)=>{CommunicateEachFrame.Stop();};
            Activated += (s, e) => { ((MainWindowViewModel)DataContext).SetWindowActive(true); };
            Deactivated  += (s, e) => { ((MainWindowViewModel)DataContext).SetWindowActive(false); };
        }

        readonly string UILayoutPath = "uiLayout.xml";
        public void LayoutLoad()
        {
            if (!File.Exists(UILayoutPath))
            {
                return;
            }
            ((MainWindowViewModel)DataContext).RestoreViewModels(UILayoutPath);
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(_dockingManager);

            using (StreamReader reader = new StreamReader(UILayoutPath))
            {
                layoutSerializer.Deserialize(reader);
            }
            var contents = new List<Xceed.Wpf.AvalonDock.Layout.LayoutContent>();
            RecursiveLayoutGetElements(_dockingManager.Layout, contents);
            contents.ForEach(
                layout =>
                {
                    layout.Closed += (_, __) =>
                    {
                        ((MainWindowViewModel)Application.Current.MainWindow.DataContext).RemoveDockingPanel(layout.Content.GetType());
                    };
                }
             );
        }
        private void RecursiveLayoutGetElements(Xceed.Wpf.AvalonDock.Layout.ILayoutContainer arg_layout,List< Xceed.Wpf.AvalonDock.Layout.LayoutContent> arg_outputContents)
        {
            if (arg_layout.ChildrenCount == 0)
            {
                return;
            }

            arg_layout.Children.ToList().ForEach(layout =>
            {
                if (layout.GetType().IsSubclassOf(typeof(Xceed.Wpf.AvalonDock.Layout.LayoutContent)))
                {
                    arg_outputContents.Add((Xceed.Wpf.AvalonDock.Layout.LayoutContent)layout);
                }
                if (layout.GetType().GetInterfaces().Contains(typeof(Xceed.Wpf.AvalonDock.Layout.ILayoutContainer)))
                {
                    RecursiveLayoutGetElements((Xceed.Wpf.AvalonDock.Layout.ILayoutContainer)layout, arg_outputContents);
                }
            });
        }
        public void LayoutSave()
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(_dockingManager);
            using (StreamWriter writer = new StreamWriter(UILayoutPath))
            {
                layoutSerializer.Serialize(writer);
            }
        }
        private void LayoutSave_Click(object sender, RoutedEventArgs e)
        {
            LayoutSave();
        }
        private void LayoutLoad_Click(object sender, RoutedEventArgs e)
        {
            LayoutLoad();
        }
        private void WindowCreate_SceneController(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<SceneControllerViewModel>();
        }
        private void WindowCreate_Console(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<ConsoleViewModel>();
        }
        private void WindowCreate_Error(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<ErrorListPaneViewModel>();
        }
        private void WindowCreate_Resource(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<ResourceLoadViewModel>();
        }
        private void WindowCreate_MaterialCreate(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<MaterialCreateViewModel>();
        }
        private void WindowCreate_FPSMonitor(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<FPSMonitorViewModel>();
        }
        private void WindowCreate_SceneViewer(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<SceneViewerViewModel>();
        }
        private void WindowCreate_ProjectSettings(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingDocument<ProjectSettingDocumentViewModel>();
        }
        private void WindowCreate_RenderTargetCreate(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<RenderTargetCreateViewModel>();
        }
        private void WindowCreate_Inspector(object sender, RoutedEventArgs e)
        {
            //((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<SceneControllerViewModel>();
        }
        private void WindowCreate_Hierarchy(object sender, RoutedEventArgs e)
        {
            //((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<SceneControllerViewModel>();
        }
        private void WindowCreate_ButiScriptCompiler(object sender, RoutedEventArgs e)
        {
            //((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<SceneControllerViewModel>();
        }
        private void WindowCreate_ShaderCreater(object sender, RoutedEventArgs e)
        {
            //((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<SceneControllerViewModel>();
        }
        private void WindowCreate_HLSLCompiler(object sender, RoutedEventArgs e)
        {
            //((MainWindowViewModel)Application.Current.MainWindow.DataContext).AddDockingPane<SceneControllerViewModel>();
        }
    }
}

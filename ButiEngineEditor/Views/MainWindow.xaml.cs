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
            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            CommunicateEachFrame.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CommunicateEachFrame.Start();
        }

        readonly string UILayoutPath = "uiLayout.xml";
        public void LayoutLoad()
        {
            if (!File.Exists(UILayoutPath))
            {
                return;
            }
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(_dockingManager);
            using (StreamReader reader = new StreamReader(UILayoutPath))
            {
                layoutSerializer.Deserialize(reader);
            }
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
    }
}

using ButiEngineEditor.ViewModels.Panes;
using System;
using System.Collections.Generic;
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

namespace ButiEngineEditor.Views.Panes
{
    /* 
     * If some events were receive from ViewModel, then please use PropertyChangedWeakEventListener and CollectionChangedWeakEventListener.
     * If you want to subscribe custome events, then you can use LivetWeakEventListener.
     * When window closing and any timing, Dispose method of LivetCompositeDisposable is useful to release subscribing events.
     *
     * Those events are managed using WeakEventListener, so it is not occurred memory leak, but you should release explicitly.
     */
    public partial class FPSMonitorPane : UserControl
    {
        public FPSMonitorPane()
        {
            InitializeComponent();
            Loaded += FPSMonitorPane_Loaded;
            Unloaded += FPSMonitorPane_Unloaded;
        }

        private void FPSMonitorPane_Unloaded(object sender, RoutedEventArgs e)
        {
            FPSMonitorUpdateStop();
        }

        private void FPSMonitorPane_Loaded(object sender, RoutedEventArgs e)
        {
            FPSMonitorUpdateStart();
        }

        public void FPSMonitorUpdateStart()
        {
            ((FPSMonitorViewModel)DataContext).UpdateStart(FPSText,Dispatcher);
        }
        public void FPSMonitorUpdateStop()
        {
            ((FPSMonitorViewModel)DataContext).UpdateStop();
        }

    }
}
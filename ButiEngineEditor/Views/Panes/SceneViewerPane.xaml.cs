using ButiEngineEditor.Models;
using ButiEngineEditor.Models.Modules;
using ButiEngineEditor.ViewModels.Panes;
using System.Windows;
using System.Windows.Controls;

namespace ButiEngineEditor.Views.Panes
{
    /* 
     * If some events were receive from ViewModel, then please use PropertyChangedWeakEventListener and CollectionChangedWeakEventListener.
     * If you want to subscribe custome events, then you can use LivetWeakEventListener.
     * When window closing and any timing, Dispose method of LivetCompositeDisposable is useful to release subscribing events.
     *
     * Those events are managed using WeakEventListener, so it is not occurred memory leak, but you should release explicitly.
     */
    public partial class SceneViewerPane : UserControl
    {

        public SceneViewerPane()
        {
            InitializeComponent();
            Loaded += SceneViewerPane_Loaded;
            Unloaded += SceneViewerPane_Unloaded;
            SceneImage.Effect =new RBFlipShader();
        }

        private void SceneViewerPane_Unloaded(object sender, RoutedEventArgs e)
        {
            ((SceneViewerViewModel)DataContext).RenderTargetUpdateStop();
        }

        private void SceneViewerPane_Loaded(object sender, RoutedEventArgs e)
        {
            ((SceneViewerViewModel)DataContext).RenderTargetUpdateStart(SceneImage,Dispatcher);
        }

    }
}
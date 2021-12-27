using ButiEngineEditor.Models;
using ButiEngineEditor.Models.Modules;
using ButiEngineEditor.ViewModels.Panes;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
        BitmapSource src = null;
        public SceneViewerPane()
        {
            InitializeComponent();
            Loaded += SceneViewerPane_Loaded;
            ((SceneViewerViewModel)DataContext).model.ViewStart();
            SceneImage.Effect =new RBFlipShader();
        }

        private void SceneViewerPane_Loaded(object sender, RoutedEventArgs e)
        {
            ImageUpdateStart();
        }

        private void SceneImageUpdate()
        {
            BitmapSource src =null;
            ((SceneViewerViewModel)DataContext).model.GetRTVData(ref src);
            SceneImage.Source = src;
        }
        private async void ImageUpdateStart()
        {
            await Task.Run(() => {

                var m = new SceneViewerModel();
                while (m.IsView)
                {
                    System.Threading.Thread.Sleep(5);
                    this.Dispatcher.Invoke((Action)(()=> {
                        m.GetRTVData(ref src);
                        SceneImage.Source = src;

                    }));
                }
            }
            );
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            SceneImageUpdate();
        }
    }
}
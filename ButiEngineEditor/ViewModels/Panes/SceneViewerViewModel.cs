
using ButiEngineEditor.Models;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class SceneViewerViewModel : PaneViewModelBase
    {
        public RenderTargetViewerModel model;
        Byte[] src = null;
        #region Title Property
        public override string Title
        {
            get { return "SceneView"; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "SceneViewerViewModel"; }
        }
        #endregion

        public SceneViewerViewModel()
        {
            model = EditorInstances.SceneViewerModel;
        }
        public void Initialize()
        {

        }
        public void RenderTargetUpdateStop()
        {
            model.ViewEnd();
        }
        public void RenderTargetUpdateStart(Image arg_image,Dispatcher arg_dispatcher)
        {
            model.ViewStart();
            model.GetRTDataStream((data) => {
                if (data.Length > 0)
                {
                    arg_dispatcher.Invoke(() => {
                        arg_image.Source = BitmapSource.Create(model.RTInfo.width, model.RTInfo.height, 96, 96, model.RTInfo.format, null, data, model.RTInfo.stride);
                    }); 
                    CommunicateEachFrame.Update();
                }
                
            });
        }
    }
}

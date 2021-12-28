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
using System.Windows.Controls;
using System.Windows.Threading;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class FPSMonitorViewModel : PaneViewModelBase
    {
        private FPSMonitorModel _fpsMonitorModel;
        public FPSMonitorModel Model { get { return _fpsMonitorModel; } }
        public void Initialize()
        {
            _fpsMonitorModel = EditorInstances.FPSMonitorModel;
        }

        public void UpdateStart(TextBlock arg_text, Dispatcher arg_dispatcher)
        {
            CommunicateEachFrame.PushActions("FPSMonitor",()=>{
                Model.Update();
                arg_dispatcher.Invoke(() => {
                    arg_text.Text = "Average:" + Model.AverageFPS.ToString() + "\nCurrent:" + Model.CurrentFPS.ToString();
                });
            });
        }
        public void UpdateStop()
        {
            CommunicateEachFrame.PopActions("FPSMonitor");
        }
        #region Title Property
        public override string Title
        {
            get { return "FPSMonitor"; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "FPSMonitorViewModel"; }
        }
        #endregion
    }
}

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

namespace ButiEngineEditor.ViewModels.Panes
{
    public class FPSMonitorViewModel : PaneViewModelBase
    {
        private FPSMonitorModel _fpsMonitorModel;
        public FPSMonitorModel FPSMonitorModel { get { return _fpsMonitorModel; } }
        public void Initialize()
        {
            _fpsMonitorModel = EditorInstances.FPSMonitorModel;
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

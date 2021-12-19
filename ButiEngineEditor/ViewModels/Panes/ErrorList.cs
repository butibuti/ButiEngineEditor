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
    public class ErrorListPaneViewModel : PaneViewModelBase
    {
        #region Title Property
        public override string Title
        {
            get { return "エラー一覧"; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "ErrorListPaneViewModel"; }
        }
        #endregion
    }
}

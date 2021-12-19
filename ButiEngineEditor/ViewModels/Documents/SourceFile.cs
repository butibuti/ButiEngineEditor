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

namespace ButiEngineEditor.ViewModels.Documents
{
    public class SourceFileDocumentViewModel : DocumentViewModelBase
    {
        #region Title Property
        public override string Title
        {
            get { return "Source.cs"; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "SourceFileDocumentViewModel"; }   // 実際はファイルのフルパス等を使用するべき
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;


namespace ButiEngineEditor.ViewModels.Documents
{
    public abstract class DocumentViewModelBase : ViewModel
    {
        #region Title Property
        /// <summary>
        /// ウィンドウまたはタブのタイトル
        /// </summary>
        public abstract string Title { get; }
        #endregion

        #region ContentId Property
        /// <summary>
        /// Document を一意に識別するための値 (レイアウトの保存等で使用する)
        /// </summary>
        public abstract string ContentId { get; }
        #endregion
        public virtual bool CanClose { get { return true;} }
    }
}

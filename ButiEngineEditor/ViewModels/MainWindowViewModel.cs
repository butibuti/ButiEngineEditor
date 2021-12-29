using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using ButiEngineEditor.Models;
using System.Windows.Input;

namespace ButiEngineEditor.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region DockingDocumentViewModels 変更通知プロパティ
        private ObservableCollection<ViewModel> _DockingDocumentViewModels;
        /// <summary>
        /// ドッキングドキュメントの ViewModel のリスト
        /// </summary>
        public ObservableCollection<ViewModel> DockingDocumentViewModels
        {
            get { return _DockingDocumentViewModels; }
            set
            {
                if (_DockingDocumentViewModels == value)
                    return;
                _DockingDocumentViewModels = value;
                RaisePropertyChanged(ref _DockingDocumentViewModels, () => DockingDocumentViewModels);
            }
        }
        #endregion

        #region DockingPaneViewModels 変更通知プロパティ
        private ObservableCollection<ViewModel> _DockingPaneViewModels;
        /// <summary>
        /// ドッキングペインの ViewModel のリスト
        /// </summary>
        public ObservableCollection<ViewModel> DockingPaneViewModels
        {
            get { return _DockingPaneViewModels; }
            set
            {
                if (_DockingPaneViewModels == value)
                    return;
                _DockingPaneViewModels = value;
                RaisePropertyChanged(ref _DockingPaneViewModels,() => DockingPaneViewModels);
            }
        }
        #endregion

        private Models.MainWindowModel mainWindowModel;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            DockingDocumentViewModels = new ObservableCollection<ViewModel>();
            DockingDocumentViewModels.Add(new ViewModels.Documents.SourceFileDocumentViewModel());
            DockingDocumentViewModels.Add(new ViewModels.Documents.ProjectSettingDocumentViewModel());

            DockingPaneViewModels = new ObservableCollection<ViewModel>();
            DockingPaneViewModels.Add(new ViewModels.Panes.ErrorListPaneViewModel());
            DockingPaneViewModels.Add(new ViewModels.Panes.OutputPaneViewModel());
            DockingPaneViewModels.Add(new ViewModels.Panes.SceneControllerViewModel());
            DockingPaneViewModels.Add(new ViewModels.Panes.FPSMonitorViewModel());
            DockingPaneViewModels.Add(new ViewModels.Panes.SceneViewerViewModel());
            mainWindowModel = new MainWindowModel();

            var t=Models.Modules.ButiEngineIO.MessageStream();
        }

        /// <summary>
        /// ウィンドウの初期化処理
        /// </summary>
        public void Initialize()
        {
        }
        protected override void Dispose(bool arg_disposing)
        {
            Models.Modules.ButiEngineIO.MessageStreamStop();
            Models.Modules.ButiEngineIO.ShutDown();
        }
        public void SetWindowActive(bool arg_active)
        {
            Models.Modules.ButiEngineIO.SetWindowActive(arg_active);
        }
    }
}

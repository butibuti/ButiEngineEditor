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
using ButiEngineEditor.ViewModels.Documents;
using System.Windows.Input;
using System.Xml;
using ButiEngineEditor.ViewModels.Panes;

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
                RaisePropertyChanged(ref _DockingPaneViewModels, () => DockingPaneViewModels);
            }
        }
        #endregion

        Dictionary<string,Func<ViewModel> > dic_paneCreate;
        Dictionary<string,Func<ViewModel> > dic_docCreate;
        private Dictionary<Type,ViewModel> dic_uniqueViewModels;
        private Models.MainWindowModel mainWindowModel;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            mainWindowModel = new MainWindowModel();
            dic_uniqueViewModels = new Dictionary<Type, ViewModel>();
            DockingDocumentViewModels = new ObservableCollection<ViewModel>();
            DockingPaneViewModels = new ObservableCollection<ViewModel>();
            dic_docCreate = new Dictionary<string, Func<ViewModel>>();
            dic_paneCreate = new Dictionary<string, Func<ViewModel>>();
            void RegistDocType<T>(bool arg_isUnique) where T : DocumentViewModelBase, new()
            {
                dic_docCreate.Add(new T().ContentId, () => { return new T(); });
                if (arg_isUnique)
                {
                    dic_uniqueViewModels.Add(typeof(T),null);
                }
            }
            void RegistPaneType<T>(bool arg_isUnique) where T : PaneViewModelBase, new()
            {
                dic_paneCreate.Add(new T().ContentId, () => { return new T(); });
                if (arg_isUnique)
                {
                    dic_uniqueViewModels.Add(typeof(T),null);
                }
            }

            RegistDocType<SourceFileDocumentViewModel>(false);
            RegistDocType<ProjectSettingDocumentViewModel>(true);

            RegistPaneType<ErrorListPaneViewModel>(true);
            RegistPaneType<ConsoleViewModel>(true);
            RegistPaneType<SceneControllerViewModel>(true);
            RegistPaneType<FPSMonitorViewModel>(true);
            RegistPaneType<SceneViewerViewModel>(true);
            RegistPaneType<ResourceLoadViewModel>(true);
            RegistPaneType<MaterialCreateViewModel>(true);
            RegistPaneType<RenderTargetCreateViewModel>(true);
            RegistPaneType<ShaderCreateViewModel>(true);
            RegistPaneType<HLSLCompilerViewModel>(true);
            RegistPaneType<ButiScriptCompilerViewModel>(true);

            var t=Models.Modules.ButiEngineIO.MessageStream();
        }

        /// <summary>
        /// ウィンドウの初期化処理
        /// </summary>
        public void Initialize()
        {
        }
        public void RestoreViewModels(string arg_uiLayoutFilePath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(arg_uiLayoutFilePath);
            var nodes = xml.SelectNodes("//LayoutAnchorable");

            var xmlNodes = nodes.Cast<XmlNode>().ToArray();
            nodes = xml.SelectNodes("//LayoutDocument");
            var xn= xmlNodes.Concat( nodes.Cast<XmlNode>().ToArray());
            foreach (var node in xn)
            {
                if (dic_paneCreate.ContainsKey(node.Attributes["ContentId"].Value))
                {
                    AddDockingPane(dic_paneCreate[node.Attributes["ContentId"].Value]());
                }
                if (dic_docCreate.ContainsKey(node.Attributes["ContentId"].Value))
                {
                    AddDockingDocument(dic_docCreate[node.Attributes["ContentId"].Value]());
                }
            }
        }
        public Type AddDockingPane<Type>() where Type : ViewModel, new()
        {
            Type newVM;
            if (dic_uniqueViewModels.ContainsKey(typeof(Type)))
            {
                if (dic_uniqueViewModels[typeof(Type)] != null)
                {
                    return null;
                }
                newVM = (new Type());
                dic_uniqueViewModels[typeof(Type)] = newVM;
            }
            else
            {
                newVM = (new Type());
            }
            DockingPaneViewModels.Add(newVM);
            return newVM;
        }
        public ViewModel AddDockingPane(ViewModel arg_VM) 
        {
            if (dic_uniqueViewModels.ContainsKey(arg_VM.GetType()))
            {
                if (dic_uniqueViewModels[arg_VM.GetType()] != null)
                {
                    return null;
                }
                dic_uniqueViewModels[arg_VM.GetType()] = arg_VM;
            }
            _DockingPaneViewModels.Add(arg_VM);
            return arg_VM;
        }
        public Type AddDockingDocument<Type>() where Type : ViewModel, new()
        {
            Type newVM;
            if (dic_uniqueViewModels.ContainsKey(typeof(Type)))
            {
                if (dic_uniqueViewModels[typeof(Type)] != null)
                {
                    return null;
                }
                newVM = (new Type());
                dic_uniqueViewModels[typeof(Type)] = newVM;
            }
            else
            {
                newVM = (new Type());
            }
            DockingDocumentViewModels.Add(newVM);
            return newVM;
        }
        public ViewModel AddDockingDocument(ViewModel arg_VM)
        {
            if (dic_uniqueViewModels.ContainsKey(arg_VM.GetType()))
            {
                if (dic_uniqueViewModels[arg_VM.GetType()] != null)
                {
                    return null;
                }
                dic_uniqueViewModels[arg_VM.GetType()] = arg_VM;
            }
            DockingDocumentViewModels.Add(arg_VM);
            return arg_VM;
        }
        public void RemoveDockingPanel<T>()
        {
            if (dic_uniqueViewModels[typeof(T)] != null)
            {
                dic_uniqueViewModels[typeof(T)] = null;
            }
        }
        public void RemoveDockingPanel(Type arg_type)
        {
            if (dic_uniqueViewModels[arg_type] != null)
            {
                dic_uniqueViewModels[arg_type] = null;
            }
        }
        public bool Contains_pane<T>()
        {
            var panes = DockingPaneViewModels.Where(vm => { return vm.GetType().Equals(typeof(T)); }).ToArray();

            return panes.Length != 0;
        }
        public bool Contains_document<T>()
        {
            var docs = DockingDocumentViewModels.Where(vm => { return vm.GetType().Equals(typeof(T)); }).ToArray();

            return docs.Length != 0;
        }
        public bool Contains_unique<T>()
        {
            return dic_uniqueViewModels[typeof(T)] != null;
        }
        public bool Contains_unique(Type arg_type)
        {
            return dic_uniqueViewModels[arg_type] != null;
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

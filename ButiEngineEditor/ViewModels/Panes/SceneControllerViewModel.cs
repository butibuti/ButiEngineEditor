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
using System.Windows.Input;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class SceneControllerViewModel : PaneViewModelBase
    {
        private Models.SceneControllerModel sceneController;
        private PropertyChangedEventListener sceneActiveListener;
        #region Title Property
        public override string Title
        {
            get { return "シーン情報"; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "SceneControllerViewModel"; }
        }
        #endregion
        public bool isActive { get; set; }
        public SceneControllerViewModel()
        {
        }

        public void Initialize()
        {
            sceneController = EditorInstances.SceneControllerModel;
            sceneActiveListener = new PropertyChangedEventListener(sceneController)
            {
                ()=>sceneController.IsActive, (_, __) => PlayButtonChange(sceneController.IsActive)
            };
            isActive = false;
        }



        private void PlayButtonChange(bool arg_isActive)
        {
            isActive = arg_isActive;
        }

        private ViewModelCommand reloadClick;
        private ViewModelCommand saveClick;

        public ICommand ReloadClick
        {
            get
            {
                if (reloadClick == null)
                {
                    reloadClick = new ViewModelCommand(PerformReloadClick);
                }

                return reloadClick;
            }
        }
        public ICommand SaveClick
        {
            get
            {
                if (saveClick == null)
                {
                    saveClick = new ViewModelCommand(PerformSaveClick);
                }
                return saveClick;
            }
        }


        public void PerformPlayClick()
        {
            sceneController.SetSceneActive(!isActive);
        }
        public void PerformReloadClick()
        {
            sceneController.SceneReload();
            isActive = false;
        }
        private void PerformSaveClick()
        {
            if (!isActive)
            {
                sceneController.SceneSave();
            }
        }
        public void PerformSceneChange(string arg_sceneChangeName)
        {
            sceneController.SceneChange(arg_sceneChangeName);
        }

    }
}

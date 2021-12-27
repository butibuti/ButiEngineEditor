using ButiEngineEditor.Models.Modules;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ButiEngineEditor.Models
{
    public class ButiEngineCommunicate : NotificationObject
    {

    }
    public class FPSMonitorModel : NotificationObject
    {
        private float _currentFPS;
        private float _averageFPS;
        private int  _drawTime;
        private int _updateTime;
        public float CurrentFPS { get { return _currentFPS; } }
        public float AverageFPS { get { return _averageFPS; } }
        public int DrawTime{ get { return _drawTime; } }
        public int UpdateTime{ get { return _updateTime; } }

        public void Update()
        {
            Modules.ButiEngineIO.GetFPS(ref _currentFPS,ref _averageFPS,ref _drawTime,ref _updateTime);
        }

    }
    public class SceneViewerModel : NotificationObject 
    {

        private static readonly string SceneRTVName = ":/NormalBuffer/1920/1080/28";
        private RenderTargetViewInformation _renderTargetViewInformation;
        private static bool isViewd = false;
        public bool IsView { get { return isViewd; } }
        private RenderTargetViewInformation RTVInfo { get { if (_renderTargetViewInformation == null) { _renderTargetViewInformation = ButiEngineIO.GetRenderTargetInformation(SceneRTVName); } return _renderTargetViewInformation; } }

        private string _sceneName;
        public string SceneName { get { return _sceneName; } }
        public SceneViewerModel()
        {
            _sceneName = "DefaultScene";
        }
        public void ViewStart()
        {
            if (!isViewd)
            {
                isViewd = ButiEngineIO.SetRenderTargetViewedByEditor(SceneRTVName, true);
            }
        }
        public void ViewEnd()
        {
            if (isViewd)
            {
                isViewd = ButiEngineIO.SetRenderTargetViewedByEditor(SceneRTVName, false);
            }
        }
        public void GetRTVData(ref BitmapSource arg_bitmapSrc)
        {
            var buff = ButiEngineIO.GetRenderTargetData(SceneRTVName, RTVInfo);
            arg_bitmapSrc = FormatConvertedBitmap.Create(RTVInfo.width, RTVInfo.height, 96, 96, RTVInfo.format, null, buff.Result, RTVInfo.stride);

        }
    }

    public class SceneControllerModel : NotificationObject
    {
        private string currentSceneName;

        private bool _isActive;
        public bool IsActive
        {
            get
            { return _isActive; }
            set
            { 
                if (_isActive == value)
                    return;
                _isActive = value;
                RaisePropertyChanged();
            }
        }

        public void SetSceneActive(bool arg_isActive)
        {
            IsActive= Modules.ButiEngineIO.SetSceneActive(arg_isActive);
        }

        public void SceneReload()
        {
            Modules.ButiEngineIO.SceneReload();
            IsActive = false;
        }
        public void SceneSave()
        {
            Modules.ButiEngineIO.SceneSave();
        }
        public void SceneChange(string arg_sceneChangeName)
        {
            Modules.ButiEngineIO.SceneChange(arg_sceneChangeName);
        }

    }

}

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
            ButiEngineIO.GetFPS(ref _currentFPS,ref _averageFPS,ref _drawTime,ref _updateTime);
        }
    }


    public class RenderTargetViewerModel : NotificationObject 
    {
        private string _renderTargetName ;
        public string RenderTargetName { get { return _renderTargetName; } }
        private RenderTargetInformation _renderTargetViewInformation;
        private bool isViewd = false;
        public bool IsView { get { return isViewd; } }
        public RenderTargetInformation RTInfo { get { if (_renderTargetViewInformation == null) { _renderTargetViewInformation = ButiEngineIO.GetRenderTargetInformation(RenderTargetName); } return _renderTargetViewInformation; } }

        public RenderTargetViewerModel(string arg_renderTargetName)
        {
            _renderTargetName = arg_renderTargetName;
        }
        public void ViewStart()
        {
            if (!isViewd)
            {
                isViewd = ButiEngineIO.SetRenderTargetViewedByEditor(_renderTargetName, true);
            }
        }
        public void ViewEnd()
        {
            if (isViewd)
            {
                isViewd = ButiEngineIO.SetRenderTargetViewedByEditor(_renderTargetName, false);
            }
        }
        public void GetRTData(ref Byte[] arg_bitmapAry)
        {
            arg_bitmapAry= ButiEngineIO.GetRenderTargetData(_renderTargetName, RTInfo).Result;

        }
        public void GetRTData(ref BitmapSource arg_bitmapSrc)
        {
            var buff = ButiEngineIO.GetRenderTargetData(_renderTargetName, RTInfo);
            arg_bitmapSrc = FormatConvertedBitmap.Create(RTInfo.width, RTInfo.height, 96, 96, RTInfo.format, null, buff.Result, RTInfo.stride);

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

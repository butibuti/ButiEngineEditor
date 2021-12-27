using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButiEngineEditor.Models
{
    class EditorInstances
    {
        private static SceneControllerModel _sceneControllerModel;
        private static ProjectSettingsModel _projectSettingsModel;
        private static FPSMonitorModel _fpsMonitorModel;
        private static SceneViewerModel _sceneViewerModel;
        public static SceneControllerModel SceneControllerModel { get { if (_sceneControllerModel == null) { _sceneControllerModel = new SceneControllerModel(); } return _sceneControllerModel; } }
        public static ProjectSettingsModel ProjectSettingsModel { get { if (_projectSettingsModel == null) { _projectSettingsModel = new ProjectSettingsModel(App.GetArgments()[0]); }   return _projectSettingsModel; } }
        public static FPSMonitorModel FPSMonitorModel { get { if (_fpsMonitorModel == null) { _fpsMonitorModel = new FPSMonitorModel(); }   return _fpsMonitorModel; } }
        public static SceneViewerModel sceneViewerModel { get { if (_sceneViewerModel == null) { _sceneViewerModel= new SceneViewerModel(); }   return _sceneViewerModel; } }
    }
}

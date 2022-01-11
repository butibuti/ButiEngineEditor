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
        private static RenderTargetViewerModel _sceneViewerModel;
        private static ResourceLoadModel _resourceLoadModel;
        private static MaterialCreateModel _materialCreateModel;
        private static RenderTargetCreateModel _RTCreateModel;
        private static ShaderCreateModel _shaderCreateModel;
        private static ButiScriptCompilerModel _butiScriptCompilerModel;
        private static HLSLCompilerModel _hlslCompilerModel;
        private static InspectorModel _inspectorModel;
        private static HierarchyModel _hierarchyModel;
        private static CollisionSettingsModel _collisionSettingsModel;
        private static RenderingSettingsModel _renderingSettingsModel;
        public static SceneControllerModel SceneControllerModel { get { if (_sceneControllerModel == null) { _sceneControllerModel = new SceneControllerModel(); } return _sceneControllerModel; } }
        public static ProjectSettingsModel ProjectSettingsModel { get { if (_projectSettingsModel == null) { _projectSettingsModel = new ProjectSettingsModel(App.GetArgments()[0]); }   return _projectSettingsModel; } }
        public static FPSMonitorModel FPSMonitorModel { get { if (_fpsMonitorModel == null) { _fpsMonitorModel = new FPSMonitorModel(); }   return _fpsMonitorModel; } }
        public static RenderTargetViewerModel SceneViewerModel { get { if (_sceneViewerModel == null) { _sceneViewerModel= new RenderTargetViewerModel(":/_editorScreen/1920/1080"); }   return _sceneViewerModel; } }
        public static ResourceLoadModel ResourceLoadModel { get { if (_resourceLoadModel== null) { _resourceLoadModel= new ResourceLoadModel(ProjectSettingsModel.GetProjFilePathDirectory()+"ResourceLoadData.json");_resourceLoadModel.FileInput(); }   return _resourceLoadModel; } }
        public static MaterialCreateModel MaterialCreateModel { get { if (_materialCreateModel == null) { _materialCreateModel = new MaterialCreateModel(); }   return _materialCreateModel; } }
        public static RenderTargetCreateModel RenderTargetCreateModel { get { if (_RTCreateModel== null) { _RTCreateModel= new RenderTargetCreateModel(); }   return _RTCreateModel; } }
        public static ShaderCreateModel ShaderCreateModel { get { if (_shaderCreateModel == null) { _shaderCreateModel = new ShaderCreateModel(); }   return _shaderCreateModel; } }
        public static ButiScriptCompilerModel ButiScriptCompilerModel { get { if (_butiScriptCompilerModel == null) { _butiScriptCompilerModel = new ButiScriptCompilerModel(); }   return _butiScriptCompilerModel; } }
        public static HLSLCompilerModel HLSLCompilerModel { get { if (_hlslCompilerModel == null) { _hlslCompilerModel = new HLSLCompilerModel(); }   return _hlslCompilerModel; } }
        public static InspectorModel InspectorModel { get { if (_inspectorModel == null) { _inspectorModel = new InspectorModel(); }   return _inspectorModel; } }
        public static HierarchyModel HierarchyModel { get { if (_hierarchyModel == null) { _hierarchyModel = new HierarchyModel(); }   return _hierarchyModel; } }
        public static CollisionSettingsModel CollisionSettingsModel { get { if (_collisionSettingsModel == null) { _collisionSettingsModel = new CollisionSettingsModel(); }   return _collisionSettingsModel; } }
        public static RenderingSettingsModel RenderingSettingsModel { get { if (_renderingSettingsModel == null) { _renderingSettingsModel = new RenderingSettingsModel(); }   return _renderingSettingsModel; } }
    }
    class CommunicateEachFrame
    {
        private static bool _isActive;
        public static bool IsActive { get { return _isActive; } }
        private static Dictionary<string,Action> dictionary_actions=new Dictionary<string, Action>();
        public static void PushActions(string arg_key, Action arg_act)
        {
            dictionary_actions.Add(arg_key, arg_act);
        }
        public static void PopActions(string arg_key)
        {
            dictionary_actions.Remove(arg_key);
        }
        public static void Start()
        {
            _isActive = true;
        }
        public static void Update()
        {
            if (!IsActive)
            {
                return;
            }
            dictionary_actions.Values.ToList().ForEach(action => { action(); });
        }
        public static void Stop()
        {
            _isActive = false;
            dictionary_actions.Clear();
        }
    }
}

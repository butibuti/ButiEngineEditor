using ButiEngineEditor.Models;
using ButiEngineEditor.Views;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using static ButiEngineEditor.Models.ResourceLoadModel;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class ResourceLoadViewModel : PaneViewModelBase
    {
        public class FilePathData: INotifyPropertyChanged
        {
            private string _filePath, _title;
            public string FilePath { get => _filePath; set 
                {
                    if (value == _filePath)
                        return;
                    _filePath = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(_filePath));
                } }
            public string Title { get => _title; set 
                {
                    if (value == _title)
                        return;
                    _title = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(_title));
                } }
            public event PropertyChangedEventHandler PropertyChanged;
            private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class TextureData : FilePathData  {}
        public class ScriptData : FilePathData  {}
        public class SoundData : FilePathData  {}
        public class ModelData : FilePathData  {}
        public class MotionData : FilePathData  { }
        public class FontData : FilePathData  { }
        public class ShaderData: INotifyPropertyChanged
        {
            private string _geometryShader,_pixelShader,_vertexShader;
            public string ShaderName { get; set; }
            public string VertexShader
            {
                get => _vertexShader;
                set
                {
                    if (value == _vertexShader)
                        return;
                    _vertexShader = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(_vertexShader));
                }
            }
            public string PixelShader
            {
                get => _pixelShader;
                set
                {
                    if (value == _pixelShader)
                        return;
                    _pixelShader = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(_pixelShader));
                }
            }
            public string GeometryShader {
                get => _geometryShader;
                set{
                    if (value == _geometryShader)
                        return;
                    _geometryShader = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(_geometryShader));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class VerexShaderData: FilePathData { }
        public class PixelShaderData: FilePathData { }
        public class GeometryShaderData: FilePathData { }
        public class MaterialData
        {
            public string MaterialName { get; set; }
        }


        public ObservableCollection<TextureData> textures = new ObservableCollection<TextureData>();
        public ObservableCollection<TextureData> renderTargetTextures = new ObservableCollection<TextureData>();
        public ObservableCollection<MotionData> motions = new ObservableCollection<MotionData>();
        public ObservableCollection<ModelData> models = new ObservableCollection<ModelData>();
        public ObservableCollection<FontData> fonts = new ObservableCollection<FontData>();
        public ObservableCollection<SoundData> sounds = new ObservableCollection<SoundData>();
        public ObservableCollection<ScriptData> scripts = new ObservableCollection<ScriptData>();
        public ObservableCollection<VerexShaderData> vShaders = new ObservableCollection<VerexShaderData>();
        public ObservableCollection<PixelShaderData> pShaders = new ObservableCollection<PixelShaderData>();
        public ObservableCollection<GeometryShaderData> gShaders = new ObservableCollection<GeometryShaderData>();
        public ObservableCollection<ShaderData> shaders = new ObservableCollection<ShaderData>();
        public ObservableCollection<MaterialData> materials = new ObservableCollection<MaterialData>();
        private PropertyChangedEventListener materialListListener,renderTargetListner,shaderListListner;
        ResourceLoadModel _model;
        ResourceLoadModel Model { get { if (_model == null) { _model = EditorInstances.ResourceLoadModel; } return _model; } }
        public override string Title { get { return "ResourceLoad"; } }

        public override string ContentId { get { return "ResourceLoadViewModel"; } }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public void Initialize()
        {
            void CollectionInit<T>(List<string> arg_src, ObservableCollection<T> arg_initList,string arg_baseDir) where T:FilePathData,new()
            {
               arg_src.ForEach(title => {
                    arg_initList.Add(new T() { Title = title, FilePath = Path.Combine(arg_baseDir, title) });
                });
            }
            string resourceAbsPath=EditorInstances.ProjectSettingsModel.GetResourceAbsoluteDirectory();
            CollectionInit(Model.Data.List_textures, textures,resourceAbsPath);
            CollectionInit(Model.Data.List_renderTargets, renderTargetTextures,"");
            CollectionInit(Model.Data.List_models, models, resourceAbsPath);
            CollectionInit(Model.Data.List_motions, motions, resourceAbsPath);
            CollectionInit(Model.Data.List_sounds, sounds,resourceAbsPath);
            CollectionInit(Model.Data.List_scripts, scripts,resourceAbsPath);
            CollectionInit(Model.Data.List_fonts, fonts,resourceAbsPath);
            CollectionInit(Model.Data.List_vertexShaders, vShaders,resourceAbsPath);
            CollectionInit(Model.Data.List_pixelShaders, pShaders,resourceAbsPath);
            CollectionInit(Model.Data.List_geometryShaders, gShaders,resourceAbsPath);
            Model.Data.List_materials.ForEach(data => {
                materials.Add(new MaterialData() { MaterialName = data.materialName });
            });
            Model.Data.List_shaders.ForEach(data => {
                shaders.Add(new ShaderData() { ShaderName = data.ShaderName });
            });
            materialListListener= new PropertyChangedEventListener(_model)
            {
                ()=>_model.MaterialAddition, (_, __) => {
                    materials.Clear();
                    Model.Data.List_materials.ForEach(data => {
                        materials.Add(new MaterialData() { MaterialName = data.materialName });
                    }); 
                }
            };
            renderTargetListner=new PropertyChangedEventListener(_model)
            {
                ()=>_model.RenderTargetAddition, (_, __) => {
                    renderTargetTextures.Clear();
                    Model.Data.List_renderTargets.ForEach(data => {
                        renderTargetTextures.Add(new TextureData() { FilePath = data,Title=data });
                    });
                }
            };
            shaderListListner = new PropertyChangedEventListener(_model) {
                ()=>_model.ShaderAddition, (_, __) => {
                    shaders.Clear();
                    Model.Data.List_shaders.ForEach(data => {
                        shaders.Add(new ShaderData() { ShaderName = data.ShaderName });
                    });
                }
            };

        }
        public void Save()
        {
            Model.FileOutput();
        }
        public void LoadPath<T>(string path, List<string> dataList, ObservableCollection<T> viewList) where T : FilePathData, new()
        {
            if (!File.Exists(path))
            {
                return;
            }
            string title = new Uri(EditorInstances.ProjectSettingsModel.GetResourceAbsoluteDirectory()).MakeRelativeUri(new Uri(path)).ToString();

            title = title.Replace('/', '\\');
            if (dataList.Exists(s => s == title))
            {
                return;
            }
            dataList.Add(title);
            viewList.Add(new T() { FilePath = path, Title = title });
            Save();
        }
        public void UnLoadPath<T>(string path, List<string> dataList, ObservableCollection<T> viewList) where T : FilePathData, new()
        {
            string title = new Uri(EditorInstances.ProjectSettingsModel.GetResourceAbsoluteDirectory()).MakeRelativeUri(new Uri(path)).ToString();
            title = title.Replace('/', '\\');
            if (!dataList.Exists(s => s == title))
            {
                return;
            }
            dataList.Remove(title);
            var temp = viewList.Where(fd => fd.Title == title);
            foreach (var t in temp)
            {
                viewList.Remove(t);
                break;
            }
            Save();
        }
        public void LoadTexture(string path)
        {
            if (!path.Contains(".png") && !path.Contains(".PNG"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_textures, textures);
        }
        public void LoadRenderTarget(string path)
        {
            if (Model.Data.List_renderTargets.Exists(s => s == path))
            {
                return;
            }
            Model.Data.List_renderTargets.Add(path);
            renderTargetTextures.Add(new TextureData() { FilePath = path, Title = path });

            Save();
        }
        public void LoadMaterial(MaterialLoadInfo arg_material)
        {
            if (Model.Data.List_materials.Exists(s => s.materialName == arg_material.materialName))
            {
                return;
            }
            Model.Data.List_materials.Add(arg_material);
            materials.Add(new MaterialData() { MaterialName = arg_material.materialName });
            Save();
        }
        public void LoadModel(string path)
        {
            if (!path.Contains(".b3m"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_models, models);
        }
        public void LoadMotion(string path)
        {
            if (!path.Contains(".bmd"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_motions, motions);
        }
        public void LoadSound(string path)
        {
            if (!path.Contains(".wav"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_sounds, sounds);
        }
        public void LoadScript(string path)
        {
            if (!path.Contains(".bs"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_scripts, scripts);
        }
        public void LoadFont(string path)
        {
            if (!path.Contains(".ttc"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_fonts, fonts);
        }
        public void LoadVertexShader(string path)
        {
            if (!path.Contains(".dx12cps"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_vertexShaders, vShaders);
        }
        public void LoadPixelShader(string path)
        {
            if (!path.Contains(".dx12cps"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_pixelShaders, pShaders);
        }
        public void LoadGeometryShader(string path)
        {
            if (!path.Contains(".dx12cps"))
            {
                return;
            }
            LoadPath(path, Model.Data.List_geometryShaders, gShaders);
        }
        public void UnLoadTexture(string path)
        {
            UnLoadPath(path, Model.Data.List_textures, textures);
        }
        public void UnLoadRenderTargetTexture(string path)
        {
            if (!Model.Data.List_renderTargets.Exists(s => s == path))
            {
                return;
            }
            Model.Data.List_renderTargets.Remove(path);
            var temp = renderTargetTextures.Where(fd => fd.Title == path);
            foreach (var t in temp)
            {
                renderTargetTextures.Remove(t);
                break;
            }

            Save();
        }
        public void UnLoadShader(string shaderName)
        {
            if (!Model.Data.List_shaders.Exists(s => s.ShaderName == shaderName))
            {
                return;
            }
            {
                var temp = Model.Data.List_shaders.Where(sd => sd.ShaderName == shaderName); 
                foreach (var t in temp)
                {
                    Model.Data.List_shaders.Remove(t);
                    break;
                }
            }
            {
                var temp = shaders.Where(sd => sd.ShaderName == shaderName);
                foreach (var t in temp)
                {
                    shaders.Remove(t);
                    break;
                }
            }
            Save();
        }
        public void UnLoadMaterial(string materialName)
        {
            if (!Model.Data.List_materials.Exists(s => s.materialName== materialName))
            {
                return;
            }
            {
                var temp = Model.Data.List_materials.Where(md => md.materialName == materialName);
                foreach (var t in temp)
                {
                    Model.Data.List_materials.Remove(t);
                    break;
                }
            }
            {
                var temp = materials.Where(md => md.MaterialName == materialName);
                foreach (var t in temp)
                {
                    materials.Remove(t);
                    break;
                }
            }
            Save();
        }
        public void UnLoadSound(string path)
        {
            UnLoadPath(path, Model.Data.List_sounds, sounds);
        }
        public void UnLoadModel(string path)
        {
            UnLoadPath(path, Model.Data.List_models, models);
        }
        public void UnLoadMotion(string path)
        {
            UnLoadPath(path, Model.Data.List_motions, motions);
        }
        public void UnLoadFont(string path)
        {
            UnLoadPath(path, Model.Data.List_fonts, fonts);
        }
        public void UnLoadScript(string path)
        {
            UnLoadPath(path, Model.Data.List_scripts, scripts);
        }
        public void UnLoadVertexShader(string path)
        {
            UnLoadPath(path, Model.Data.List_vertexShaders, vShaders);
        }
        public void UnLoadPixelShader(string path)
        {
            UnLoadPath(path, Model.Data.List_pixelShaders, pShaders);
        }
        public void UnLoadGeometryShader(string path)
        {
            UnLoadPath(path, Model.Data.List_geometryShaders, gShaders);
        }
    }
}

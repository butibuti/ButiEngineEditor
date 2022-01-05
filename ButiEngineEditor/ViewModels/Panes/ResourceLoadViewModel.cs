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
using System.Text;
using System.Windows.Media;
using static ButiEngineEditor.Models.ResourceLoadModel;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class ResourceLoadViewModel : PaneViewModelBase
    {
        public class FilePathData
        {
            public string FilePath { get; set; }
            public string Title { get; set; }
        }
        public class ShaderData
        {
            public string ShaderName { get; set; }
        }
        public class MaterialData
        {
            public string MaterialName { get; set; }
        }


        public ObservableCollection<FilePathData> textures = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> renderTargetTextures = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> motions = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> models = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> fonts = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> sounds = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> scripts = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> vShaders = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> pShaders = new ObservableCollection<FilePathData>();
        public ObservableCollection<FilePathData> gShaders = new ObservableCollection<FilePathData>();
        public ObservableCollection<ShaderData> shaders = new ObservableCollection<ShaderData>();
        public ObservableCollection<MaterialData> materials = new ObservableCollection<MaterialData>();
        private PropertyChangedEventListener materialListListener;
        ResourceLoadModel _model;
        ResourceLoadModel Model { get { if (_model == null) { _model = EditorInstances.ResourceLoadModel; } return _model; } }
        public override string Title { get { return "ResourceLoad"; } }

        public override string ContentId { get { return "ResourceLoadViewModel"; } }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public void Initialize()
        {
            void CollectionInit(List<string> arg_src,  ObservableCollection<FilePathData> arg_initList,string arg_baseDir)
            {
               arg_src.ForEach(title => {
                    arg_initList.Add(new FilePathData() { Title = title, FilePath = Path.Combine(arg_baseDir, title) });
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
        }
        public void Save()
        {
            Model.FileOutput();
        }
        public void LoadPath(string path, List<string> dataList, ObservableCollection<FilePathData> viewList)
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
            viewList.Add(new FilePathData() { FilePath = path, Title = title });
            Save();
        }
        public void UnLoadPath(string path, List<string> dataList, ObservableCollection<FilePathData> viewList)
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
            renderTargetTextures.Add(new FilePathData() { FilePath = path, Title = path });

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

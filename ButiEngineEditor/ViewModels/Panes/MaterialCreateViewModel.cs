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
using System.Windows.Controls;
using System.Windows.Media;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class MaterialCreateViewModel : PaneViewModelBase
    {
        public class TextureDataForMaterialCreate: ResourceLoadViewModel.TextureData
        {
            public CustomDropHandler DropHandler { get; set; }
        }

        private ObservableCollection<TextureDataForMaterialCreate> _textures = new ObservableCollection<TextureDataForMaterialCreate>();
        public ObservableCollection<TextureDataForMaterialCreate> Textures { get { return _textures; } }
        private CustomDropHandler _textureListCustomDropHandler;
        public CustomDropHandler TextureListCustomDropHandler 
        { 
            get {
                if (_textureListCustomDropHandler == null)
                {
                    _textureListCustomDropHandler = new CustomDropHandler((arg_dropInfo) => {
                        if (Textures.First().FilePath == "Dummy")
                        {
                            Textures.Clear();
                        }
                        Textures.Add(new TextureDataForMaterialCreate() { DropHandler= TextureListCustomDropHandler, Title = ((ResourceLoadViewModel.TextureData)arg_dropInfo.Data).Title, FilePath = ((ResourceLoadViewModel.TextureData)arg_dropInfo.Data).FilePath });
                        ModelUpdate();
                    }, typeof(ResourceLoadViewModel.TextureData));
                }
                return _textureListCustomDropHandler;
            }  
        }
        private ResourceLoadModel ResourceLoadModel { get { return EditorInstances.ResourceLoadModel;  } }
        private MaterialCreateModel MaterialCreateModel { get { return EditorInstances.MaterialCreateModel; } }
        public Color Diffuse { get { return MaterialCreateModel.Diffuse; } set { MaterialCreateModel.Diffuse = value; } }
        public Color Ambient  { get { return MaterialCreateModel.Ambient; } set { MaterialCreateModel.Ambient= value; } }
        public Color Emissive { get { return MaterialCreateModel.Emissive; } set { MaterialCreateModel.Emissive= value; } }
        public Color Specular { get { return MaterialCreateModel.Specular; } set { MaterialCreateModel.Specular = value; } }
        public string MaterialName { get { return MaterialCreateModel.MaterialName; } set { MaterialCreateModel.MaterialName = value; } }
        public override string Title { get { return "MaterialCreate"; } }

        public override string ContentId { get { return "MaterialCreateViewModel"; } }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public MaterialCreateViewModel()
        {
            MaterialCreateModel.List_currentSelectTextures.ForEach(td => { Textures.Add(new TextureDataForMaterialCreate() { DropHandler = TextureListCustomDropHandler, Title = td.Title, FilePath = td.FilePath }); });
            CreateTextureDummy();
        }
        public void Initialize()
        {
        }
        public void Save()
        {
            ResourceLoadModel.FileOutput();
        }
        public void ModelUpdate()
        {
            MaterialCreateModel.List_currentSelectTextures.Clear();
            foreach(var t in Textures)
            {
                if (t.FilePath == "Dummy")
                {
                    continue;
                }
                MaterialCreateModel.List_currentSelectTextures.Add(new ResourceLoadViewModel.TextureData() { Title = t.Title, FilePath = t.FilePath });
            }
        }
        public void CreateTextureDummy()
        {
            if (Textures.Count == 0)
            {
                Textures.Add(new TextureDataForMaterialCreate() { DropHandler = TextureListCustomDropHandler, Title = "Drag Texture...", FilePath = "Dummy" });
            }
        }


        public void CreateMaterial()
        {
            if (ResourceLoadModel.Data.List_materials.Exists(md=>md.materialName==MaterialName))
            {
                return;
            }

            ResourceLoadModel.MaterialValue materialValue = new ResourceLoadModel.MaterialValue();
            materialValue.diffuse = new System.Numerics.Vector4() { X = Diffuse.R * (1.0f / 255), Y = Diffuse.G * (1.0f / 255), Z = Diffuse.B * (1.0f / 255), W = Diffuse.A * (1.0f / 255) };
            materialValue.ambient = new System.Numerics.Vector4() { X = Ambient.R * (1.0f / 255), Y = Ambient.G * (1.0f / 255), Z = Ambient.B * (1.0f / 255), W = Ambient.A * (1.0f / 255) };
            materialValue.emissive = new System.Numerics.Vector4() { X = Emissive.R * (1.0f / 255), Y = Emissive.G * (1.0f / 255), Z = Emissive.B * (1.0f / 255), W = Emissive.A * (1.0f / 255) };
            materialValue.specular = new System.Numerics.Vector4() { X = Specular.R * (1.0f / 255), Y = Specular.G * (1.0f / 255), Z = Specular.B * (1.0f / 255), W = Specular.A * (1.0f / 255) };

            ResourceLoadModel.Data.List_materials.Add(new ResourceLoadModel.MaterialLoadInfo() { materialName = MaterialName, var = materialValue });
            ResourceLoadModel.MaterialAddition=true;
            Save();
        }

        public void UnLoadTexture(string arg_title)
        {
            var temp = Textures.Where(fd => fd.Title == arg_title);
            foreach (var t in temp)
            {
                Textures.Remove(t);
                break;
            }
            ModelUpdate();
            CreateTextureDummy();
        }
    }
}

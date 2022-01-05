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

namespace ButiEngineEditor.ViewModels.Panes
{
    public class MaterialCreateViewModel : PaneViewModelBase
    {

        public ObservableCollection<ResourceLoadViewModel. FilePathData> textures = new ObservableCollection<ResourceLoadViewModel.FilePathData>();
        public TextBoxCustomDropHandler TextBoxCustomDropHandler { get; set; } = new TextBoxCustomDropHandler();
        ResourceLoadModel _model;
        ResourceLoadModel Model { get { if (_model == null) { _model = EditorInstances.ResourceLoadModel; } return _model; } }
        public override string Title { get { return "MaterialCreate"; } }

        public override string ContentId { get { return "MaterialCreateViewModel"; } }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public MaterialCreateViewModel()
        {
        }
        public void Initialize()
        {
        }
        public void Save()
        {
            Model.FileOutput();
        }


        public void CreateMaterial(string arg_materialName, Color arg_diffuse, Color arg_ambient, Color arg_emissive, Color arg_specular)
        {
            ResourceLoadModel.MaterialValue materialValue = new ResourceLoadModel.MaterialValue();
            materialValue.diffuse = new System.Numerics.Vector4() { X = arg_diffuse.R * (1.0f / 255), Y = arg_diffuse.G * (1.0f / 255), Z = arg_diffuse.B * (1.0f / 255), W = arg_diffuse.A * (1.0f / 255) };
            materialValue.ambient = new System.Numerics.Vector4() { X = arg_ambient.R * (1.0f / 255), Y = arg_ambient.G * (1.0f / 255), Z = arg_ambient.B * (1.0f / 255), W = arg_ambient.A * (1.0f / 255) };
            materialValue.emissive = new System.Numerics.Vector4() { X = arg_emissive.R * (1.0f / 255), Y = arg_emissive.G * (1.0f / 255), Z = arg_emissive.B * (1.0f / 255), W = arg_emissive.A * (1.0f / 255) };
            materialValue.specular = new System.Numerics.Vector4() { X = arg_specular.R * (1.0f / 255), Y = arg_specular.G * (1.0f / 255), Z = arg_specular.B * (1.0f / 255), W = arg_specular.A * (1.0f / 255) };

            Model.Data.List_materials.Add(new ResourceLoadModel.MaterialLoadInfo() { materialName = arg_materialName, var = materialValue });
            Model.MaterialAddition=true;
            Model.FileOutput();
        }
    }
}

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
    public class ShaderCreateViewModel : PaneViewModelBase
    {
        private CustomDropHandler _vertexCustomDropHandler,_pixelCustomDropHandler,_geometryCustomDropHandler;
        public CustomDropHandler VertexCustomDropHandler
        {
            get
            {
                if (_vertexCustomDropHandler == null)
                {
                    _vertexCustomDropHandler = new CustomDropHandler((arg_dropInfo) => {
                        VertexShader.Title = ((ResourceLoadViewModel.VerexShaderData)arg_dropInfo.Data).Title;
                    }, typeof(ResourceLoadViewModel.VerexShaderData));
                }
                return _vertexCustomDropHandler;
            }
        }
        public CustomDropHandler PixelCustomDropHandler
        {
            get
            {
                if (_pixelCustomDropHandler == null)
                {
                    _pixelCustomDropHandler = new CustomDropHandler((arg_dropInfo) => {
                        PixelShader.Title = ((ResourceLoadViewModel.PixelShaderData)arg_dropInfo.Data).Title;
                    }, typeof(ResourceLoadViewModel.PixelShaderData));
                }
                return _pixelCustomDropHandler;
            }
        }
        public CustomDropHandler GeometryCustomDropHandler
        {
            get
            {
                if (_geometryCustomDropHandler == null)
                {
                    _geometryCustomDropHandler = new CustomDropHandler((arg_dropInfo) => {
                        GeometryShader.Title = ((ResourceLoadViewModel.GeometryShaderData)arg_dropInfo.Data).Title;
                    }, typeof(ResourceLoadViewModel.GeometryShaderData));
                }
                return _geometryCustomDropHandler;
            }
        }
        private ResourceLoadModel ResourceLoadModel { get { return EditorInstances.ResourceLoadModel; } }
        private ShaderCreateModel ShaderCreateModel { get { return EditorInstances.ShaderCreateModel; } }
        public string ShaderName { get { return ShaderCreateModel.ShaderName; } set { ShaderCreateModel.ShaderName = value; } }
        public override string Title { get { return "ShaderCreate"; } }

        public override string ContentId { get { return "ShaderCreateViewModel"; } }

        public ResourceLoadViewModel.VerexShaderData VertexShader { get { return ShaderCreateModel.VertexShader; } set { ShaderCreateModel.VertexShader = value; } }
        public ResourceLoadViewModel.PixelShaderData PixelShader { get { return ShaderCreateModel.PixelShader; } set { ShaderCreateModel.PixelShader= value;  } }
        public ResourceLoadViewModel.GeometryShaderData GeometryShader { get { return ShaderCreateModel.GeometryShader; } set { ShaderCreateModel.GeometryShader = value; } }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public ShaderCreateViewModel()
        {

        }
        public void Initialize()
        {
        }
        public void Save()
        {
            ResourceLoadModel.FileOutput();
        }


        public void CreateShader()
        {
            if (ResourceLoadModel.Data.List_shaders.Exists(md=>md.ShaderName==ShaderName)||VertexShader.Title=="None"||PixelShader.Title=="None")
            {
                return;
            }
            ResourceLoadModel.Data.List_shaders.Add(new ResourceLoadModel.ShaderLoadInfo() { ShaderName = this.ShaderName,VertexShaderName=VertexShader.Title,PixelShaderName=PixelShader.Title, GeometryShaderName = GeometryShader.Title });

            ResourceLoadModel.ShaderAddition=true;
            Save();
        }

    }
}

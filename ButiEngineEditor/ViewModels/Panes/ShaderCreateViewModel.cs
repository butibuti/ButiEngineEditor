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
        private CustomDropHandler _vertexListCustomDropHandler;
        public CustomDropHandler VertexListCustomDropHandler 
        { 
            get {
                if (_vertexListCustomDropHandler == null)
                {
                }
                return _vertexListCustomDropHandler;
            }
        }
        private ResourceLoadModel ResourceLoadModel { get { return EditorInstances.ResourceLoadModel; } }
        private ShaderCreateModel ShaderCreateModel { get { return EditorInstances.ShaderCreateModel; } }
        public string ShaderName { get { return ShaderCreateModel.ShaderName; } set { ShaderCreateModel.ShaderName = value; } }
        public override string Title { get { return "ShaderCreate"; } }

        public override string ContentId { get { return "ShaderCreateViewModel"; } }

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
            if (ResourceLoadModel.Data.List_shaders.Exists(md=>md.ShaderName==ShaderName))
            {
                return;
            }

            ResourceLoadModel.ShaderAddition=true;
            Save();
        }

    }
}

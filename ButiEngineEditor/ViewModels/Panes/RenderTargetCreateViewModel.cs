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
    public class RenderTargetCreateViewModel : PaneViewModelBase
    {

        private ResourceLoadModel ResourceLoadModel { get { return EditorInstances.ResourceLoadModel; } }
        private RenderTargetCreateModel RenderTargetCreateModel { get { return EditorInstances.RenderTargetCreateModel; } }
        public string RenderTargetName { get { return RenderTargetCreateModel.RenderTargetName; } set { RenderTargetCreateModel.RenderTargetName = value; } }
        public int Width { get { return RenderTargetCreateModel.Width; } set { RenderTargetCreateModel.Width = value; } }
        public int Height { get { return RenderTargetCreateModel.Height; } set { RenderTargetCreateModel.Height = value; } }
        public override string Title { get { return "RenderTargetCreate"; } }

        public override string ContentId { get { return "RenderTargetCreateViewModel"; } }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public RenderTargetCreateViewModel()
        {
        }
        public void Initialize()
        {
        }
        public void Save()
        {
            ResourceLoadModel.FileOutput();
        }



        public void CreateRenderTarget(RenderTargetFormat arg_format)
        {
            string path = ":/" + RenderTargetName + "/" + Width + "/" + Height + "/" + ((int)arg_format).ToString();
            if (ResourceLoadModel.Data.List_renderTargets.Exists(rt=>rt== path))
            {
                return;
            }

            ResourceLoadModel.Data.List_renderTargets.Add(path);
            ResourceLoadModel.RenderTargetAddition=true;
            Save();
        }
    }
}

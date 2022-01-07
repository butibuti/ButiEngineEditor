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
    public class HLSLCompilerViewModel : PaneViewModelBase
    {
        private ResourceLoadModel ResourceLoadModel { get { return EditorInstances.ResourceLoadModel; } }
        private HLSLCompilerModel HLSLCompilerModel { get { return EditorInstances.HLSLCompilerModel; } }
        public string FilePath { get { return HLSLCompilerModel.FilePath; } set { HLSLCompilerModel.FilePath= value; } }
        public override string Title { get { return "HLSLCompiler"; } }

        public override string ContentId { get { return "HLSLCompilerViewModel"; } }

        public HLSLCompilerViewModel()
        {

        }
        public void Initialize()
        {
        }
        public void Save()
        {
            ResourceLoadModel.FileOutput();
        }


        public void Compile(string arg_filePath)
        {
        }
        public void Load(string arg_title)
        {
        }

    }
}

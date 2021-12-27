using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using ButiEngineEditor.Models;

namespace ButiEngineEditor.ViewModels.Documents
{
    public class ProjectSettingDocumentViewModel : DocumentViewModelBase
    {

        public ProjectSettingsModel ProjectSettings { get; set; }
        public ProjectSettingDocumentViewModel()
        {
            ProjectSettings = EditorInstances.ProjectSettingsModel;
        }
        public void Initialize()
        {
        }
        #region Title Property
        public override string Title
        {
            get { return ProjectSettings.projFilePath; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "ProjectSettingDocumentViewModel"; }
        }
        #endregion
    }
}

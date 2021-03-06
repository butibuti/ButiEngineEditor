using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ButiEngineEditor.Views.Documents
{
    /// <summary>
    /// ProjectSettingDocument.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectSettingDocument : UserControl
    {
        public ProjectSettingDocument()
        {
            InitializeComponent();
            Loaded += TextBoxInit;
        }

        private void TextBoxInit(object sender, RoutedEventArgs e)
        {
            ProjectNameBox.Text = ((ViewModels.Documents.ProjectSettingDocumentViewModel)DataContext).ProjectSettings.projectName;
        }

        private void ProjectSettingsUpdate(object sender, RoutedEventArgs e)
        {
            ((ViewModels.Documents.ProjectSettingDocumentViewModel)DataContext).ProjectSettings.projectName = ProjectNameBox.Text;
            ((ViewModels.Documents.ProjectSettingDocumentViewModel)DataContext).ProjectSettings.FileOutput();

        }
    }
}

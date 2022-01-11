using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Livet.Messaging;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ButiEngineEditor.ViewModels.Panes;
using ButiEngineEditor.Models.Modules;
using Microsoft.WindowsAPICodePack.Dialogs;
using ButiEngineEditor.Models;

namespace ButiEngineEditor.Views.Panes
{
    /* 
     * If some events were receive from ViewModel, then please use PropertyChangedWeakEventListener and CollectionChangedWeakEventListener.
     * If you want to subscribe custome events, then you can use LivetWeakEventListener.
     * When window closing and any timing, Dispose method of LivetCompositeDisposable is useful to release subscribing events.
     *
     * Those events are managed using WeakEventListener, so it is not occurred memory leak, but you should release explicitly.
     */
    public partial class SceneControllerPane : UserControl
    {
        public SceneControllerPane()
        {
            InitializeComponent();
        }
        public void IconChange(bool arg_isActive)
        {
            if (arg_isActive)
            {
                ((MahApps.Metro.IconPacks.PackIconEntypo)PlayButton.Content).Kind = MahApps.Metro.IconPacks.PackIconEntypoKind.ControllerStop;
                SaveButton.Foreground=new SolidColorBrush(Color.FromRgb(120,120,120));
            }
            else
            {
                ((MahApps.Metro.IconPacks.PackIconEntypo)PlayButton.Content).Kind = MahApps.Metro.IconPacks.PackIconEntypoKind.ControllerPlay;
                SaveButton.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            
        }


        private void IconCheck(object sender, RoutedEventArgs e)
        {
            IconChange(((SceneControllerViewModel)DataContext).isActive);

        }
        private void ReloadClick(object sender, RoutedEventArgs e)
        {
            ((SceneControllerViewModel)DataContext).PerformReloadClick();
            IconChange(((SceneControllerViewModel)DataContext).isActive);

        }
        private void PlayClick(object sender, RoutedEventArgs e)
        {
            ((SceneControllerViewModel)DataContext).PerformPlayClick();
            IconChange(((SceneControllerViewModel)DataContext).isActive);

        }

        private void SceneChange_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();

            dlg.IsFolderPicker = true;
            dlg.Title = "シーンフォルダを選択してください";
            dlg.InitialDirectory = EditorInstances.ProjectSettingsModel.GetResourceAbsoluteDirectory()+@"Scene\";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var selected = dlg.FileName.Replace(dlg.InitialDirectory,"").Replace("\\","");

                ((SceneControllerViewModel)DataContext).PerformSceneChange(selected);
                IconChange(((SceneControllerViewModel)DataContext).isActive);
            }
        }

        private void AppStart_Click(object sender, RoutedEventArgs e)
        {
            ButiEngineIO.ApplicationStartUp();
        }

        private void AppShutDown_Click(object sender, RoutedEventArgs e)
        {
            ButiEngineIO.ApplicationShutDown(-1);
        }
    }
}
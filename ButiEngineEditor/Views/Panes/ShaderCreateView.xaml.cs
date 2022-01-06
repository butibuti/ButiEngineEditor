using ButiEngineEditor.Models;
using ButiEngineEditor.ViewModels.Panes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ButiEngineEditor.Views.Panes
{
    public partial class ShaderCreateView : UserControl
    {
        private CollectionViewSource textureView = new CollectionViewSource();
        public ShaderCreateView()
        {
            InitializeComponent();
            textureView.Source = ((MaterialCreateViewModel)DataContext).Textures;
            Textures.DataContext = textureView;
        }

        private void MaterialCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialNameBox.Text.Length < 1) { return; }
            ((MaterialCreateViewModel)DataContext).CreateMaterial();
        }
        private void TextureDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Textures.SelectedItems.Count < 1)
            {
                return;
            }
            var selected = Textures.SelectedItems.Cast<ResourceLoadViewModel.FilePathData>();
            var list_remPath = new List<string>();
            foreach (ResourceLoadViewModel.FilePathData deletePath in selected)
            {
                if (deletePath.FilePath != "Dummy")
                {
                    list_remPath.Add(deletePath.Title);
                }
            }
            list_remPath.ForEach(s => ((MaterialCreateViewModel)DataContext).UnLoadTexture(s));
        }
    }
}
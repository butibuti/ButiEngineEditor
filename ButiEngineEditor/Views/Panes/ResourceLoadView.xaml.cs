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
    public static class ButtonExtensions
    {
        public static void PerformClick(this Button button)
        {
            if (button == null)
                throw new ArgumentNullException("button");

            var provider = new ButtonAutomationPeer(button) as IInvokeProvider;
            provider.Invoke();
        }
    }
    [ValueConversion(typeof(bool), typeof(bool))]
    internal class BoolToInvertedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack() of BoolToInvertedBoolConverter is not implemented");
        }
    }
    public enum RenderTargetFormat
    {
        R8G8B8A8_UNORM=28, R32G32B32A32_FLOAT=2, R32G32B32_FLOAT=6, R32G32_FLOAT=16, R32_FLOAT= 41
    }
    public class ComnboBoxShowData<Type> { 
        public Type Code { get; set; }
        public string Name { get; set; }
    }

    public class EnumSourceProvider<T> : MarkupExtension
    {
        private static string DisplayName(T value)
        {
            var fileInfo = value.GetType().GetField(value.ToString());

            if(fileInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() == null)
            {
                return value.ToString();
            }
            else
            {
                var descriptionAttribute = (DescriptionAttribute)fileInfo
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault();
                return descriptionAttribute.Description;
            }

        }

        public IEnumerable Source { get; }
            = typeof(T).GetEnumValues()
                .Cast<T>()
                .Select(value => new ComnboBoxShowData<T> { Code = value, Name = DisplayName(value) });

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
    public class RTFormatSourceProvider : EnumSourceProvider<RenderTargetFormat> { }
    public partial class ResourceLoadView : UserControl
    {

        private CollectionViewSource textureView = new CollectionViewSource();
        private CollectionViewSource renderTargetTextureView = new CollectionViewSource();
        private CollectionViewSource materialView = new CollectionViewSource();
        private CollectionViewSource shaderView = new CollectionViewSource();
        private CollectionViewSource modelView = new CollectionViewSource();
        private CollectionViewSource motionView = new CollectionViewSource();
        private CollectionViewSource soundView = new CollectionViewSource();
        private CollectionViewSource scriptView = new CollectionViewSource();
        private CollectionViewSource fontView = new CollectionViewSource();
        private CollectionViewSource pixelShaderView = new CollectionViewSource();
        private CollectionViewSource vertexShaderView = new CollectionViewSource();
        private CollectionViewSource geometryShaderView = new CollectionViewSource();
        public ResourceLoadView()
        {
            InitializeComponent();
            textureView.Source = ((ResourceLoadViewModel)DataContext).textures;
            modelView.Source = ((ResourceLoadViewModel)DataContext).models;
            motionView.Source = ((ResourceLoadViewModel)DataContext).motions;
            soundView.Source= ((ResourceLoadViewModel)DataContext).sounds;
            scriptView.Source= ((ResourceLoadViewModel)DataContext).scripts;
            fontView.Source= ((ResourceLoadViewModel)DataContext).fonts;
            pixelShaderView.Source= ((ResourceLoadViewModel)DataContext).pShaders;
            vertexShaderView.Source= ((ResourceLoadViewModel)DataContext).vShaders;
            geometryShaderView.Source= ((ResourceLoadViewModel)DataContext).gShaders;
            renderTargetTextureView.Source= ((ResourceLoadViewModel)DataContext).renderTargetTextures;
            materialView.Source= ((ResourceLoadViewModel)DataContext).materials;
            shaderView.Source= ((ResourceLoadViewModel)DataContext).shaders;
            TextureList.DataContext = textureView;
            RenderTargetTextureList.DataContext = renderTargetTextureView;
            MaterialList.DataContext = materialView;
            ShaderList.DataContext = shaderView;
            ModelList.DataContext = modelView;
            MotionList.DataContext = motionView;
            SoundList.DataContext = soundView;
            ScriptList.DataContext = scriptView;
            FontList.DataContext = fontView;
            PixelShaderList.DataContext = pixelShaderView;
            VertexShaderList.DataContext = vertexShaderView;
            GeometryShaderList.DataContext = geometryShaderView;
            SetDropAction(TextureList, path => ((ResourceLoadViewModel)DataContext).LoadTexture(path));
            SetDropAction(ModelList, path => ((ResourceLoadViewModel)DataContext).LoadModel(path));
            SetDropAction(MotionList, path => ((ResourceLoadViewModel)DataContext).LoadMotion(path));
            SetDropAction(SoundList, path => ((ResourceLoadViewModel)DataContext).LoadSound(path));
            SetDropAction(ScriptList, path => ((ResourceLoadViewModel)DataContext).LoadScript(path));
            SetDropAction(FontList, path => ((ResourceLoadViewModel)DataContext).LoadFont(path));
            SetDropAction(PixelShaderList, path => ((ResourceLoadViewModel)DataContext).LoadPixelShader(path));
            SetDropAction(VertexShaderList, path => ((ResourceLoadViewModel)DataContext).LoadVertexShader(path));
            SetDropAction(GeometryShaderList, path => ((ResourceLoadViewModel)DataContext).LoadGeometryShader(path));
            RenderTargetAddPopUp.LostFocus += delegate { RenderTargetAddPopUp.IsOpen = false; };
        }
        private void SetDragAction(ListView arg_list)
        {
        }
        private void SetDropAction(ListView arg_list,Action<string> arg_act)
        {
            arg_list.AllowDrop = true;
            arg_list.PreviewDragOver += (s, e) => {
                e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
                e.Handled = true;
            };
            arg_list.PreviewDrop += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var paths = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                    paths.ForEach(arg_act);
                }
            };
        }
        private void FileLoadDialog(string arg_messageTitle,List<CommonFileDialogFilter> arg_filters,string arg_initDir,Action<string> arg_loadAct)
        {
            var dlg = new CommonOpenFileDialog();

            dlg.RestoreDirectory = true;
            dlg.Multiselect = true;
            dlg.EnsureFileExists = true;
            arg_filters.ForEach(f => { dlg.Filters.Add(f); });            
            dlg.Title = arg_messageTitle;
            dlg.InitialDirectory = EditorInstances.ProjectSettingsModel.GetResourceAbsoluteDirectory() + arg_initDir;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                dlg.FileNames.ToList().ForEach(arg_loadAct);
            }
        }
        private void SelectedFileUnLoad(ListView arg_listView, Action<string> arg_unloadAct)
        {
            if (arg_listView.SelectedItems.Count < 1)
            {
                return;
            }
            var selected = arg_listView.SelectedItems.Cast<ResourceLoadViewModel.FilePathData>();
            var list_remPath = new List<string>();
            foreach (ResourceLoadViewModel.FilePathData deletePath in selected)
            {
                list_remPath.Add(deletePath.FilePath);
            }
            list_remPath.ForEach(arg_unloadAct);
        }
        private void ImageLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込む画像ファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("image file", "*.png") }, @"Texture\", fn => ((ResourceLoadViewModel)DataContext).LoadTexture(fn));
        }
        private void ImageUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(TextureList, s => ((ResourceLoadViewModel)DataContext).UnLoadTexture(s));
        }
        private void RenderTargetCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (RTNameBox.Text.Length < 1|| RTFormatBox.SelectedItem==null) {return;}

            var format =(ComnboBoxShowData<RenderTargetFormat>)  RTFormatBox.SelectedItem;
            
            ((ResourceLoadViewModel)DataContext).LoadRenderTarget(":/"+RTNameBox.Text+"/"+RTWidthBox.Value+"/"+RTHeightBox.Value + "/"+ ((int)format.Code).ToString());
        }
            private void RenderTargetUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(RenderTargetTextureList, s => ((ResourceLoadViewModel)DataContext).UnLoadRenderTargetTexture(s));
        }
        private void MaterialCreateButton_Click(object sender, RoutedEventArgs e)
        {
            //FileLoadDialog("読み込む3Dモデルファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("buti3Dmodel file", "*.b3m") }, @"Model\", fn => ((ResourceLoadViewModel)DataContext).LoadModel(fn));
        }
        private void MaterialUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialList.SelectedItems.Count < 1)
            {
                return;
            }
            var selected = MaterialList.SelectedItems.Cast<ResourceLoadViewModel.MaterialData>();
            var list_remPath = new List<string>();
            foreach (ResourceLoadViewModel.MaterialData deletePath in selected)
            {
                list_remPath.Add(deletePath.MaterialName);
            }
            list_remPath.ForEach(s => ((ResourceLoadViewModel)DataContext).UnLoadMaterial(s));
        }
        private void ModelLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込む3Dモデルファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("buti3Dmodel file", "*.b3m") }, @"Model\", fn => ((ResourceLoadViewModel)DataContext).LoadModel(fn));
        }
        private void ModelUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(ModelList, s => ((ResourceLoadViewModel)DataContext).UnLoadModel(s));
        }
        private void ShaderLoadButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void ShaderUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShaderList.SelectedItems.Count < 1)
            {
                return;
            }
            var selected = ShaderList.SelectedItems.Cast<ResourceLoadViewModel.MaterialData>();
            var list_remPath = new List<string>();
            foreach (ResourceLoadViewModel.MaterialData deletePath in selected)
            {
                list_remPath.Add(deletePath.MaterialName);
            }
            list_remPath.ForEach(s => ((ResourceLoadViewModel)DataContext).UnLoadShader(s));
        }
        private void MotionLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込むモーションファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("butiMotionData file", "*.bmd") }, @"Motion\", fn => ((ResourceLoadViewModel)DataContext).LoadMotion(fn));
        }
        private void MotionUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(MotionList, s => ((ResourceLoadViewModel)DataContext).UnLoadMotion(s));
        }

        private void SoundLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込む音声ファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("audio file", "*.wav") }, @"Sound\", fn => ((ResourceLoadViewModel)DataContext).LoadSound(fn));
        }

        private void SoundUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(SoundList, s => ((ResourceLoadViewModel)DataContext).UnLoadSound(s));
        }
        private void FontLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込むフォントファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("font file", "*.ttc") }, @"Font\", fn => ((ResourceLoadViewModel)DataContext).LoadFont(fn));
        }

        private void FontUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(FontList, s => ((ResourceLoadViewModel)DataContext).UnLoadFont(s));
        }
        private void ScriptLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込むスクリプトファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("butiScript file", "*.bs") }, @"Script\", fn => ((ResourceLoadViewModel)DataContext).LoadScript(fn));
        }

        private void ScriptUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(ScriptList, s => ((ResourceLoadViewModel)DataContext).UnLoadScript(s));
        }

        private void GeometryShaderLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込むジオメトリシェーダファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("HLSL file", "*.dx12cps") }, @"Shader\compiled\", fn => ((ResourceLoadViewModel)DataContext).LoadGeometryShader(fn));
        }

        private void GeometryShaderUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(GeometryShaderList, s => ((ResourceLoadViewModel)DataContext).UnLoadGeometryShader(s));
        }

        private void PixelShaderLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込むピクセルシェーダファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("HLSL file", "*.dx12cps") }, @"Shader\compiled\", fn => ((ResourceLoadViewModel)DataContext).LoadPixelShader(fn));
        }

        private void PixelShaderUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(PixelShaderList, s => ((ResourceLoadViewModel)DataContext).UnLoadPixelShader(s));
        }

        private void VertexShaderLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileLoadDialog("読み込む頂点シェーダファイルを選択してください", new List<CommonFileDialogFilter> { new CommonFileDialogFilter("HLSL file", "*.dx12cps") }, @"Shader\compiled\", fn => ((ResourceLoadViewModel)DataContext).LoadVertexShader(fn));
        }

        private void VertexShaderUnLoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileUnLoad(VertexShaderList, s => ((ResourceLoadViewModel)DataContext).UnLoadVertexShader(s));
        }

    }
}
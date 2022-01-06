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

    public class ComnboBoxShowData<Type>
    {
        public Type Code { get; set; }
        public string Name { get; set; }
    }

    public class EnumSourceProvider<T> : MarkupExtension
    {
        private static string DisplayName(T value)
        {
            var fileInfo = value.GetType().GetField(value.ToString());

            if (fileInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() == null)
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
    public partial class RenderTargetCreateView : UserControl
    {
        public RenderTargetCreateView()
        {
            InitializeComponent();
        }

        private void RenderTargetCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (RTNameBox.Text.Length < 1 || RTFormatBox.SelectedItem == null) { return; }

            var format = (ComnboBoxShowData<RenderTargetFormat>)RTFormatBox.SelectedItem;
            ((RenderTargetCreateViewModel)DataContext).CreateRenderTarget(format.Code);
        }
    }
}
﻿using ButiEngineEditor.Models;
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
        public ShaderCreateView()
        {
            InitializeComponent();
        }

        private void ShaderCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShaderNameBox.Text.Length < 1) { return; }
            ((ShaderCreateViewModel)DataContext).CreateShader();
        }
        private void VSDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((ShaderCreateViewModel)DataContext).VertexShader.Title = "None";
        }
        private void PSDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((ShaderCreateViewModel)DataContext).PixelShader.Title = "None";
        }
        private void GSDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((ShaderCreateViewModel)DataContext).GeometryShader.Title = "None";
        }
    }
}
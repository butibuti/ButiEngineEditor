using ButiEngineEditor.Models;
using Livet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace ButiEngineEditor
{
    public partial class App : Application
    {
        private static string[] argments;
        private readonly string DebugProjectPath = @"D:\K019G1120\Projects\ButiEngine\ButiEngine_User\SampleProject.beproj";
        private static Process _butiEngineProcess;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.UIDispatcher = Dispatcher;
            if (e.Args.Length < 1)
            {
                argments =new string[] { DebugProjectPath };
            }
            else
            {
                argments = e.Args;
            }
            
            BootRuntime();
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        public static string[] GetArgments()
        {
            return argments;
        }

        public static void SetArgment(int arg_index,string arg_argment)
        {
            if(arg_index< argments.Length)
            {
                argments[arg_index] = arg_argment;
            }
        }
        public static Process ButiEngineProcess { get { return _butiEngineProcess; } }
        public void BootRuntime()
        {
            var butiengineProcess = Process.GetProcessesByName("ButiEngine_User");
            if (butiengineProcess.Length < 1)
            {
                var commandLine =EditorInstances.ProjectSettingsModel.GetResourceAbsoluteDirectory();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = EditorInstances.ProjectSettingsModel.GetProjFilePathDirectory() + "ButiEngine_User.exe";
                processStartInfo.Arguments += commandLine;
                _butiEngineProcess =Process.Start(processStartInfo);

            }
            else
            {
                _butiEngineProcess = butiengineProcess[0];
                App.SetArgment(0, DebugProjectPath);
            }
        }
        // Application level error handling
        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    //TODO: Logging
        //    MessageBox.Show(
        //        "Something errors were occurred.",
        //        "Error",
        //        MessageBoxButton.OK,
        //        MessageBoxImage.Error);
        //
        //    Environment.Exit(1);
        //}
    }
}

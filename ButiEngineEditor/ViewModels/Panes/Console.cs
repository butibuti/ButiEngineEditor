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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class ConsoleViewModel : PaneViewModelBase
    {
        public class ConsoleMessage : System.ComponentModel.INotifyPropertyChanged
        {
            private int _count;
            public int ID { get; set; }
            public string Content { get; set; }
            public Brush Color { get; set; }
            public int Count { get { return _count; } set { _count = value; NotifyPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private ObservableCollection<ConsoleMessage> _messages = new ObservableCollection<ConsoleMessage>();
        public ObservableCollection<ConsoleMessage> Messages { get { return _messages; } }
        private Dispatcher dispatcher;
        #region Title Property
        public override string Title
        {
            get { return "Console"; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "OutputPaneViewModel"; }
        }
        #endregion

        public ConsoleViewModel()
        {
        }
        public void SetConsoleAction(Dispatcher arg_dispatcher)
        {
            this.dispatcher = arg_dispatcher;
            Models.Modules.ButiEngineIO.SetConsoleAction(PushConsoleMessage);
        }
        protected override void Dispose(bool disposing)
        {

            Models.Modules.ButiEngineIO.SetConsoleAction(null);
        }

        public void PushConsoleMessage(string arg_content, Color arg_color)
        {
            dispatcher.Invoke(() => {
                if (Messages.Count > 0 && Messages[Messages.Count - 1].Content == arg_content)
                {
                    Messages[Messages.Count - 1].Count++;
                    return;
                }
                var msg = new ConsoleMessage
                {
                    Content = arg_content,
                    Color = new SolidColorBrush(arg_color),
                    Count = 1,
                    ID = Messages.Count
                };
                Messages.Add(msg);
            });
        }
    }
}

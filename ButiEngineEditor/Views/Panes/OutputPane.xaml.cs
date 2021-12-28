using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ButiEngineEditor.Views.Panes
{
    /// <summary>
    /// OutputPane.xaml の相互作用ロジック
    /// </summary>
    public partial class OutputPane : UserControl
    {
        class ConsoleMessage {
            public string Content { get; set; }
            public Brush Color{ get; set; }
            public int Count{ get; set; }
        }
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<ConsoleMessage> customers = new ObservableCollection<ConsoleMessage>();
        public OutputPane()
        {
            InitializeComponent();
            Loaded += OutputPane_Loaded;
        }

        private void OutputPane_Loaded(object sender, RoutedEventArgs e)
        {
            ConsoleMessage msg = new ConsoleMessage();
            msg.Content = "TestMessage!!!!";
            msg.Color = new SolidColorBrush(Color.FromArgb(255,222,10,0));
            customers.Add(msg); 
            msg = new ConsoleMessage();
            msg.Content = "Green";
            msg.Color = new SolidColorBrush(Color.FromArgb(255, 22, 125, 10));
            customers.Add(msg);

            view.Source = customers;
            this.ConsoleList.DataContext = view;
        }
    }
}

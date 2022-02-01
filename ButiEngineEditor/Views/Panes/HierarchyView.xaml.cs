using ButiEngineEditor.ViewModels.Panes;
using System.ComponentModel;
using System.Linq;
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
    public partial class HierarchyView : UserControl 
    {
        private CollectionViewSource view = new CollectionViewSource();
        static int loedCount = 0;
        public HierarchyView()
        {
            InitializeComponent();
            view.Source = ((HierarchyViewModel)DataContext).List_gameObjects;
            HierarchyList.DataContext = view;
            Loaded += HierarchyView_Loaded;
            loedCount++;
            
            HierarchyList.AllowDrop = true;
            HierarchyList.PreviewDragOver += (_, e) => {
                e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
                e.Handled = true;
            };
            HierarchyList.PreviewDrop += (_, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var paths = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                    paths.ForEach(s=> {
                        if (s.Contains("json"))
                        {
                            ((HierarchyViewModel)DataContext).LoadBlenderJSON(s);
                        }
                        });
                }
            };
        }

        private void HierarchyView_Loaded(object sender, RoutedEventArgs e)
        {
            ((HierarchyViewModel)DataContext).LoadBlenderJSON("test.json");
        }
    }
}

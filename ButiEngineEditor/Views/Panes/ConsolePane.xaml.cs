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
    public partial class ConsolePane : UserControl 
    {
        private CollectionViewSource view = new CollectionViewSource();
        public ConsolePane()
        {
            InitializeComponent();
            view.Source = ((ConsoleViewModel)DataContext).Messages;
            ((ConsoleViewModel)DataContext).SetConsoleAction(Dispatcher);
            ConsoleList.DataContext = view;
            view.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Descending));
        }

        private void OrderFlip_Click(object sender, RoutedEventArgs e)
        {
            var sortDesc=view.SortDescriptions.FirstOrDefault();
            view.SortDescriptions.Clear();

            view.SortDescriptions.Add(new SortDescription("ID", sortDesc.Direction == ListSortDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending));
            ((MahApps.Metro.IconPacks.PackIconMaterial)OrderFlip.Content).Kind =(sortDesc.Direction == ListSortDirection.Descending )? MahApps.Metro.IconPacks.PackIconMaterialKind.SortClockAscending: MahApps.Metro.IconPacks.PackIconMaterialKind.SortClockDescending;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ConsoleViewModel)DataContext).Messages.Clear();
        }

    }
}

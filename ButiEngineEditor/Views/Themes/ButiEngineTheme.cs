using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Themes;

namespace ButiEngineEditor.Views.Themes
{
    class ButiEngineTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri(
                "/Views/Themes/ButiEngineTheme.xaml",
                UriKind.Relative);
        }
    }
}

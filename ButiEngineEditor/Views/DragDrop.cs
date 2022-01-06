using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ButiEngineEditor.ViewModels.Panes.ResourceLoadViewModel;

namespace ButiEngineEditor.Views
{
    public class CustomDropHandler : IDropTarget
    {
        private Type dropType;
        private Action<IDropInfo> dropAction;
        public CustomDropHandler(Action<IDropInfo> arg_action,Type arg_dropDataType)
        {
            dropAction = arg_action;
            dropType = arg_dropDataType;
        }
        public void DragEnter(IDropInfo dropInfo)
        {

        }

        public void DragLeave(IDropInfo dropInfo)
        {

        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = typeof(DropTargetHighlightAdorner);
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data.GetType().Equals(dropType))
            {
                dropAction(dropInfo);
            }
        }
    }

    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        private readonly Pen _pen;
        private readonly Brush _brush;

        public DropTargetHighlightAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement, dropInfo)
        {
            _pen = new Pen(Brushes.DeepSkyBlue, 0.5);
            _pen.Freeze();
            _brush = new SolidColorBrush(Colors.SkyBlue) { Opacity = 0.2 };
            this._brush.Freeze();

            this.SetValue(SnapsToDevicePixelsProperty, true);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var visualTarget = DropInfo.VisualTarget;
            if (visualTarget != null)
            {
                var translatePoint = visualTarget.TranslatePoint(new Point(), this.AdornedElement);
                translatePoint.Offset(1, 1);
                var bounds = new Rect(translatePoint,
                                      new Size(visualTarget.RenderSize.Width - 2, visualTarget.RenderSize.Height - 2));
                drawingContext.DrawRectangle(_brush, _pen, bounds);
            }
        }
    }
}

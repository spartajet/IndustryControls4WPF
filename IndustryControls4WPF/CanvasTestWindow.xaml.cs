using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IndustryControls4WPF
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// CanvasTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CanvasTestWindow
    {
        public CanvasTestWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.CanvasTestWindow_Loaded;
        }

        private PathFigure _figure;
        private PathGeometry _pathGeometry;
        private Path _path;

        private void CanvasTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this._path = new Path();
            this._pathGeometry = new PathGeometry();
            this._figure = new PathFigure {StartPoint = new Point(0, 0)};
            Random random=new Random();
            for (int i = 1; i < 10; i++)
            {
                LineSegment line = new LineSegment(new Point(300 * random.NextDouble(), 300 *random.NextDouble()), true);
                this._figure.Segments.Add(line);
            }

            this._pathGeometry.Figures.Add(this._figure);
            this._path.Data = this._pathGeometry;
            this._path.Stroke = Brushes.Orange;
            this.Canvas.Children.Add(this._path);
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
//            _figure.Segments.RemoveAt(4);
//            this.Canvas.Children.Clear();
//            this.Canvas.Children.Add(_path);
            LineSegment segment = this._figure.Segments[3] as LineSegment;
            if (segment != null) segment.Point = new Point(200, 200);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IndustryControls4WPF.TTL;
using System.Windows.Shapes;

namespace IndustryControls4WPF.Controls.Digital
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// TTLConfigurator.xaml 的交互逻辑
    /// </summary>
    public partial class TtlConfigurator : UserControl
    {
        public TtlConfigurator()
        {
            this.InitializeComponent();
            this.Loaded += this.TTLConfigurator_Loaded;
            
        }

        

        private int _unitHeight;
        private int _unitWidth;
        private PathFigure _figure;
        private PathGeometry _pathGeometry;
        private Path _path;

        /// <summary>
        /// 加载后执行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TTLConfigurator_Loaded(object sender, RoutedEventArgs e)
        {
            this.CellOperate();
            //绘制图形
            this.RefreshUnitSize();
            this.InitGraphicToolKit();
            this.DrawTtl();
            this.SizeChanged += TtlConfigurator_SizeChanged;
        }


        private void CellOperate()
        {
            //解析设置的码形
            char[] cs = this.TtlString.ToCharArray();
            this.TtlCells = new List<TtlCell>(cs.Length);
            for (int i = 0; i < cs.Length; i++)
            {
                char c = cs[i];
                switch (c)
                {
                    case '0':
                        this.TtlCells.Add(new TtlCell(i, TtlCellStatus.Low));
                        break;
                    case '1':
                        this.TtlCells.Add(new TtlCell(i, TtlCellStatus.High));
                        break;
                    default:
                        throw new System.IO.InvalidDataException("TTL 字符串不符合要求，请重新输入");
                }
            }
        }

        /// <summary>
        /// 刷新电平单元的长和宽
        /// </summary>
        private void RefreshUnitSize()
        {
            this._unitHeight = Convert.ToInt32(this.TtlCanvas.ActualHeight);
            this._unitWidth = Convert.ToInt32(this.TtlCanvas.ActualWidth / this.TtlCells.Count);
        }
        /// <summary>
        /// 初始化绘图工具
        /// </summary>
        private void InitGraphicToolKit()
        {
            this._path = new Path();
            this._pathGeometry = new PathGeometry();
            this._figure = new PathFigure();
        }
        /// <summary>
        /// 绘制TTL图像
        /// </summary>
        private void DrawTtl()
        {
            this._figure.Segments.Clear();
            this.TtlCanvas.Children.Clear();
            this._figure.StartPoint = this.TtlCells[0].status == TtlCellStatus.High
                ? new Point(0, 0)
                : new Point(0, this._unitHeight);
            for (int i = 0; i < this.TtlCells.Count; i++)
            {
                TtlCell cell = this.TtlCells[i];
                TtlCellStatus cellStatus = cell.status;
                int cellEndPointX = (i + 1) * this._unitWidth;
                int cellEndPointY = cellStatus == TtlCellStatus.High ? 0 : this._unitHeight;
                //画本单元的末尾点
                LineSegment unitLine =
                    new LineSegment(
                        new Point(cellEndPointX, cellEndPointY), true);
                this._figure.Segments.Add(unitLine);
                if (i >= this.TtlCells.Count - 1)
                {
                    continue;
                }
                int unitConnectLineY;
                TtlCell nextCell = this.TtlCells[i + 1];
                TtlCellStatus nextCllCellStatus = nextCell.status;
                if (cellStatus == TtlCellStatus.High && nextCllCellStatus == TtlCellStatus.Low)
                {
                    unitConnectLineY = this._unitHeight;
                }
                else if (cellStatus == TtlCellStatus.Low && nextCllCellStatus == TtlCellStatus.High)
                {
                    unitConnectLineY = 0;
                }
                else
                {
                    unitConnectLineY = cellEndPointY;
                }
                LineSegment unitConnectLine = new LineSegment(new Point(cellEndPointX,unitConnectLineY), true);
                this._figure.Segments.Add(unitConnectLine);
            }
            this._pathGeometry.Figures.Add(this._figure);
            this._path.Data = this._pathGeometry;
            this._path.Stroke = Brushes.Black;
            this.TtlCanvas.Children.Add(this._path);
        }
        /// <summary>
        /// 属性改变之后的操作
        /// </summary>
        private void PropertiesChangeOperate()
        {
            this.CellOperate();
            this.RefreshUnitSize();
            this.DrawTtl();
        }

        public List<TtlCell> TtlCells { get; set; }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(int), typeof(TtlConfigurator),
            new FrameworkPropertyMetadata(1, ThicknessPropertyCallBack));

        private static void ThicknessPropertyCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlConfigurator configurator= d as TtlConfigurator;
            configurator?.PropertiesChangeOperate();
        }

        public int Thickness
        {
            get { return (int) this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty TtlStringProperty = DependencyProperty.Register(
            "TtlString", typeof(string), typeof(TtlConfigurator),
            new FrameworkPropertyMetadata("0101010101010101", TtlStringChangeCallBack));

        private static void TtlStringChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlConfigurator configurator = d as TtlConfigurator;
            configurator?.PropertiesChangeOperate();
        }

        [Category("Data")]
        [DisplayName("TTL文本")]
        public string TtlString
        {
            get { return (string) this.GetValue(TtlStringProperty); }
            set { this.SetValue(TtlStringProperty, value); }
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
//            this.InitGraphicToolKit();
//            this.DrawTtl();
            base.OnRender(drawingContext);
        }

        private void TtlConfigurator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshUnitSize();
            this.DrawTtl();
        }

        //        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
        //            "Lenght", typeof(int), typeof(TTLConfigurator), new FrameworkPropertyMetadata(16,new PropertyChangedCallback(LengthChangedCallBack)));
        //
        //        private static void LengthChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //        {
        //            
        //        }
        //
        //        public int Length
        //        {
        //            get { return (int) GetValue(LengthProperty); }
        //            set { SetValue(LengthProperty, value); }
        //        }
    }
}
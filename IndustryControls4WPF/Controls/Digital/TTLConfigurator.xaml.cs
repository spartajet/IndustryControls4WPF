﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IndustryControls4WPF.TTL;
using System.Windows.Shapes;
using IndustryControls4WPF.Annotations;

namespace IndustryControls4WPF.Controls.Digital
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// TTLConfigurator.xaml 的交互逻辑
    /// </summary>
    public partial class TtlConfigurator : UserControl,INotifyPropertyChanged
    {
        public TtlConfigurator()
        {
            this.InitializeComponent();
            this.InitGraphicToolKit();
            this.Loaded += this.TTLConfigurator_Loaded;
        }


        private int _unitHeight;
        private int _unitWidth;
        // private PathFigure _figure;
        // private PathGeometry _pathGeometry;
        // private Path _path;
        private string _titleString="TTL Setting";
        public static ObservableCollection<TtlSection> DefaulTtlSections { get; set; } = new ObservableCollection<TtlSection>()
        {
            new TtlSection()
            {
                Length = 32,
                Status = TtlStatus.High
            },
            new TtlSection()
            {
                Length = 32,
                Status = TtlStatus.Low
            }
        };
        /// <summary>
        /// TTL 片段对象
        /// </summary>
        // public List<TtlSection> TtlSections { get; set; } = DefaulTtlSections.ToList();

        // private int _scaleInterval = 4;

        /// <summary>
        /// 加载后执行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TTLConfigurator_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.TtlSections==null||this.TtlSections.Count==0)
            {
                return;
            }
            this.CellOperate();
            //绘制图形
            this.RefreshUnitSize();
            this.DrawTtl();
            this.DrawXScale();
            this.SizeChanged += this.TtlConfigurator_SizeChanged;
        }


        private void CellOperate()
        {
            //解析设置的码形
            // char[] cs = this.TtlString.ToCharArray();
            // this.TtlCells = new List<TtlCell>(cs.Length);
            // for (int i = 0; i < cs.Length; i++)
            // {
            //     char c = cs[i];
            //     switch (c)
            //     {
            //         case '0':
            //             this.TtlCells.Add(new TtlCell(i, TtlCellStatus.Low));
            //             break;
            //         case '1':
            //             this.TtlCells.Add(new TtlCell(i, TtlCellStatus.High));
            //             break;
            //         default:
            //             throw new System.IO.InvalidDataException("TTL 字符串不符合要求，请重新输入");
            //     }
            // }
        }

        /// <summary>
        /// 刷新电平单元的长和宽
        /// </summary>
        private void RefreshUnitSize()
        {
            int totalCount = this.TtlSections.Select(t => t.Length).Sum();
            this._unitHeight = Convert.ToInt32(this.TtlCanvas.ActualHeight)-1;
            this._unitWidth = Convert.ToInt32(this.TtlCanvas.ActualWidth / totalCount);
        }

        /// <summary>
        /// 初始化绘图工具
        /// </summary>
        private void InitGraphicToolKit()
        {
            // this._path = new Path();
            // this._pathGeometry = new PathGeometry();
            // this._figure = new PathFigure();
        }

        /// <summary>
        /// 绘制TTL图像
        /// </summary>
        private void DrawTtl()
        {
            // this._figure.Segments.Clear();
            this.TtlCanvas.Children.Clear();
            Polyline polyline=new Polyline()
            {
                Stroke = Brushes.Black
            };
            // this._figure.StartPoint = this.TtlSections[0].Status == TtlStatus.High
            //     ? new Point(0, 0)
            //     : new Point(0, this._unitHeight);
            int tempCellCount = 0;
            foreach (TtlSection section in this.TtlSections)
            {
                TtlStatus sectionStatus = section.Status;
                int sectionStartX = tempCellCount * this._unitWidth;
                int sectionStartY= sectionStatus == TtlStatus.High ? 0 : this._unitHeight;
                polyline.Points.Add(new Point(sectionStartX, sectionStartY));
                tempCellCount += section.Length;
                int cellEndPointX = tempCellCount * this._unitWidth;
                int cellEndPointY = sectionStatus == TtlStatus.High ? 0 : this._unitHeight;
                polyline.Points.Add(new Point(cellEndPointX, cellEndPointY));
            }
            // this._pathGeometry.Figures.Add(this._figure);
            // this._path.Data = this._pathGeometry;
            // this._path.Stroke = Brushes.Black;
            this.TtlCanvas.Children.Add(polyline);
        }

        /// <summary>
        /// 属性改变之后的操作
        /// </summary>
        private void PropertiesChangeOperate()
        {
            this.CellOperate();
            this.RefreshUnitSize();
            this.DrawTtl();
            this.DrawXScale();
        }
        /// <summary>
        /// 绘制Scale
        /// </summary>
        private void DrawXScale()
        {
            int totalCount = this.TtlSections.Select(t => t.Length).Sum();
            for (int i = 0; i < this.BottomCanvas.Children.Count; i++)
            {
                Line line = this.BottomCanvas.Children[i] as Line;
                if (line != null && line.Name == "XLine")
                {
                    continue;
                }

                this.BottomCanvas.Children.RemoveAt(i--);
            }

            for (int i = 0; i < totalCount; i++)
            {
                int scaleHeight = (i + 1) % this.ScaleInterval == 0 ? 10 : 5;
                Line tempScaleLine = new Line
                {
                    X1 = this._unitWidth * (i + 1),
                    Y1 = 5,
                    X2 = this._unitWidth * (i + 1),
                    Y2 = 5 + scaleHeight,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                this.BottomCanvas.Children.Add(tempScaleLine);
                if ((i + 1) % this.ScaleInterval == 0)
                {
                    TextBlock scaleText = new TextBlock
                    {
                        Text = ((i + 1)).ToString(),
                        FontSize = 8
                    };
                    this.BottomCanvas.Children.Add(scaleText);
                    Canvas.SetRight(scaleText, this.ActualWidth - this._unitWidth * (i + 1)-20);
                    Canvas.SetBottom(scaleText, 3);
                }
            }
        }



        #region TTL的thickness依赖配置项(未使用)

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(int), typeof(TtlConfigurator),
            new FrameworkPropertyMetadata(1, ThicknessPropertyCallBack));

        private static void ThicknessPropertyCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlConfigurator configurator = d as TtlConfigurator;
            configurator?.PropertiesChangeOperate();
        }
        [Category("Data")]
        [DisplayName("线条粗细")]
        public int Thickness
        {
            get { return (int) this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty TtlSectionProperty = DependencyProperty.Register(
            "PropertyType", typeof(ObservableCollection<TtlSection>), typeof(TtlConfigurator), new FrameworkPropertyMetadata(defaultValue: DefaulTtlSections, TtlSectionPropertyCallBack));

        private static void TtlSectionPropertyCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlConfigurator configurator = d as TtlConfigurator;
            // configurator.TtlSections = ((ObservableCollection<TtlSection>)(e.NewValue)).ToList();
        }
        [Category("Data")]
        [DisplayName("Section")]
        public ObservableCollection<TtlSection> TtlSections
        {
            get => (ObservableCollection<TtlSection>) this.GetValue(TtlSectionProperty);
            set
            {
                this.SetValue(TtlSectionProperty, value);
                this.PropertiesChangeOperate();
            }
        }

        public static readonly DependencyProperty ScaleIntervalProperty = DependencyProperty.Register(
            "ScaleInterval", typeof(int), typeof(TtlConfigurator), new FrameworkPropertyMetadata(4,ScaleIntervalPropertyCallBack));

        private static void ScaleIntervalPropertyCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlConfigurator configurator = d as TtlConfigurator;
            // configurator.s
        }

        public int ScaleInterval
        {
            get { return (int) GetValue(ScaleIntervalProperty); }
            set { SetValue(ScaleIntervalProperty, value); }
        }

        #region TTLString依赖配置项

        #endregion

        #region 标题依赖配置项

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(TtlConfigurator),
            new FrameworkPropertyMetadata("TTL码形配置", TitleChangeCallBack));

        private static void TitleChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlConfigurator configurator = d as TtlConfigurator;
            // configurator.TitleLable.Content = configurator.Title;
            configurator.TitleString = configurator.Title;
        }

        [Category("Data")]
        [DisplayName("Title")]
        public string Title
        {
            get => (string) this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        public string TitleString
        {
            get => this._titleString;
            set
            {
                this._titleString = value;
                this.OnPropertyChanged("TitleString");
            }
        }

        #endregion


        protected override void OnRender(DrawingContext drawingContext)
        {
//            this.InitGraphicToolKit();
//            this.DrawTtl();
            base.OnRender(drawingContext);
        }

        /// <summary>
        /// 大小改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TtlConfigurator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshUnitSize();
            this.DrawTtl();
            this.DrawXScale();
        }

        /// <summary>
        /// 右击配置菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingMenu_OnClick(object sender, RoutedEventArgs e)
        {
            // TtlSettingWindow settingWindow = new TtlSettingWindow() {TtlString = this.TtlString};
            //
            // settingWindow.ShowDialog();
            // if (settingWindow.DialogResult == true)
            // {
            //     this.TtlString = settingWindow.ResultTtlString;
            // }
            TtlSignalSettingWindow ttlSignalSettingWindow=new TtlSignalSettingWindow(this.TitleString,this.TtlSections);
            ttlSignalSettingWindow.ShowDialog();
            if (ttlSignalSettingWindow.DialogResult==true)
            {
                this.TtlSections = ttlSignalSettingWindow.TtlSections;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}
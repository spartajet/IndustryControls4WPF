using System;
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
    public partial class TtlConfigurator : UserControl, INotifyPropertyChanged
    {
        public TtlConfigurator()
        {
            this.InitializeComponent();
            // this.InitGraphicToolKit();
            this.Loaded += this.TTLConfigurator_Loaded;
        }


        private double _unitHeight;

        private double _unitWidth;
        private double _minUnitWidth = 5;

        // private string _titleString = "TTL Setting";

        public static ObservableCollection<TtlStage> DefaultTtlStage { get; set; } =
            new ObservableCollection<TtlStage>()
            {
                new TtlStage()
                {
                    Name = "Delay",
                    Repeat = 1,
                    TtlSections = new List<TtlSection>(3)
                    {
                        new TtlSection()
                        {
                            Length = 2,
                            Status = TtlStatus.Low
                        },
                        new TtlSection()
                        {
                            Length = 2.5,
                            Status = TtlStatus.High
                        },
                        new TtlSection()
                        {
                            Length = 5,
                            Status = TtlStatus.Low
                        }
                    },
                },
                new TtlStage()
                {
                    Name = "Work",
                    Repeat = 3,
                    TtlSections = new List<TtlSection>(2)
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
                    }
                }
            };

        // private int _scaleInterval = 4;

        /// <summary>
        /// 加载后执行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TTLConfigurator_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.TtlStages == null || this.TtlStages.Count == 0)
            {
                return;
            }

            //绘制图形
            this.RefreshUnitSize();
            this.DrawTtl();
            this.DrawXScale();
            this.SizeChanged += this.TtlConfigurator_SizeChanged;
        }


        /// <summary>
        /// 刷新电平单元的长和宽
        /// </summary>
        private void RefreshUnitSize()
        {
            // int totalCount = this.TtlSections.Select(t => t.Length).Sum();
            double totalCount =
                this.TtlStages.Select(t => t.TtlSections.Select(t1 => t1.Length).Sum() * t.Repeat).Sum() * 2;
            this._unitWidth = Convert.ToInt32(this.TtlCanvas.ActualWidth / totalCount);
            // this._unitWidth = Convert.ToInt32((this.Width - 40) / totalCount);
            if (this._unitWidth < this._minUnitWidth)
            {
                // this.TtlCanvas.Width = this._minUnitWidth * totalCount;
                // this.BottomCanvas.Width = this._minUnitWidth * totalCount;
                this._unitWidth = this._minUnitWidth;
                this._unitHeight = Convert.ToInt32(this.TtlCanvas.ActualHeight) - 20;
            }
            else
            {
                this._unitHeight = Convert.ToInt32(this.TtlCanvas.ActualHeight) - 5;
            }
        }

        /// <summary>
        /// 绘制TTL图像
        /// </summary>
        private void DrawTtl()
        {
            this.TtlCanvas.Children.Clear();
            Polyline polyline = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            int tempCellCount = 0;
            foreach (TtlStage ttlStage in this.TtlStages)
            {
                for (int i = 0; i < ttlStage.Repeat; i++)
                {
                    foreach (TtlSection section in ttlStage.TtlSections)
                    {
                        TtlStatus sectionStatus = section.Status;
                        double sectionStartX = tempCellCount * this._unitWidth;
                        double sectionStartY = sectionStatus == TtlStatus.High ? 0 : this._unitHeight;
                        polyline.Points.Add(new Point(sectionStartX, sectionStartY));
                        tempCellCount += (int) (section.Length * 2);
                        double cellEndPointX = tempCellCount * this._unitWidth;
                        double cellEndPointY = sectionStatus == TtlStatus.High ? 0 : this._unitHeight;
                        polyline.Points.Add(new Point(cellEndPointX, cellEndPointY));
                    }
                }
            }

            this.TtlCanvas.Children.Add(polyline);
        }

        /// <summary>
        /// 属性改变之后的操作
        /// </summary>
        private void PropertiesChangeOperate()
        {
            // this.CellOperate();
            this.RefreshUnitSize();
            this.DrawTtl();
            this.DrawXScale();
        }

        /// <summary>
        /// 绘制Scale
        /// </summary>
        private void DrawXScale()
        {
            double totalCount =
                this.TtlStages.Select(t => t.TtlSections.Select(t1 => t1.Length).Sum() * t.Repeat).Sum() * 2;
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
                int scaleHeight = (i + 1) % (this.ScaleInterval * 2) == 0 ? 10 : 5;
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
                if ((i + 1) % (this.ScaleInterval * 2) == 0)
                {
                    TextBlock scaleText = new TextBlock
                    {
                        Text = ((i + 1) / 2).ToString(),
                        FontSize = 8
                    };
                    this.BottomCanvas.Children.Add(scaleText);
                    Canvas.SetTop(scaleText, 13);
                    Canvas.SetLeft(scaleText, this._unitWidth * (i + 1) - scaleText.ActualWidth);
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

        #region TtlStage

        public static readonly DependencyProperty TtlStagesTypeProperty = DependencyProperty.Register(
            "PropertyType", typeof(ObservableCollection<TtlStage>), typeof(TtlConfigurator),
            new PropertyMetadata(DefaultTtlStage));

        [Category("Data")]
        [DisplayName("Stages")]
        public ObservableCollection<TtlStage> TtlStages
        {
            get => (ObservableCollection<TtlStage>) this.GetValue(TtlStagesTypeProperty);
            set
            {
                this.SetValue(TtlStagesTypeProperty, value);
                this.PropertiesChangeOperate();
            }
        }

        #endregion

        public static readonly DependencyProperty ScaleIntervalProperty = DependencyProperty.Register(
            "ScaleInterval", typeof(int), typeof(TtlConfigurator),
            new FrameworkPropertyMetadata(4, ScaleIntervalPropertyCallBack));

        private static void ScaleIntervalPropertyCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlConfigurator configurator = d as TtlConfigurator;
            // configurator.s
        }

        public int ScaleInterval
        {
            get { return (int) GetValue(ScaleIntervalProperty); }
            set
            {
                SetValue(ScaleIntervalProperty, value);
                this.PropertiesChangeOperate();
            }
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
            // configurator.TitleString = configurator.Title;
        }

        [Category("Data")]
        [DisplayName("Title")]
        public string Title
        {
            get => (string) this.GetValue(TitleProperty);
            set
            {
                this.SetValue(TitleProperty, value);
                // this.PropertiesChangeOperate();
                this.OnPropertyChanged();
            }
        }

        #endregion


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
            NewSettingWindow newSettingWindow = new NewSettingWindow(this.Title, this.TtlStages);
            newSettingWindow.ShowDialog();
            if (newSettingWindow.DialogResult == true)
            {
                this.TtlStages = newSettingWindow.TtlStages;
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
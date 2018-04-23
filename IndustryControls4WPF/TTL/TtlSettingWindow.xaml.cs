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
using System.Windows.Shapes;

namespace IndustryControls4WPF.TTL
{
    /// <summary>
    /// TtlSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TtlSettingWindow : Window
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public TtlSettingWindow()
        {
            InitializeComponent();
            this.Loaded += TtlSettingWindow_Loaded;
        }

        public string ResultTtlString { get; set; }
        private HighLevelRegion _selectHighLevelRegion;


        /// <summary>
        /// 加载后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TtlSettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
//            this.HighRegionsListBox.DisplayMemberPath = "RegionString";
//            this.HighRegionsListBox.SelectedValuePath=""
            this.HighRegionsListBox.ItemsSource = this.HighLevelRegions;
            this.HighRegionsListBox.SelectionChanged += HighRegionsListBox_SelectionChanged;
        }

        /// <summary>
        /// List Box 选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HighRegionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            HighLevelRegion region = listBox?.SelectedValue as HighLevelRegion;
            if (region == null) return;
            this._selectHighLevelRegion = region;
        }

        /// <summary>
        /// ttl单元list
        /// </summary>
        public List<TtlCell> Cells { get; set; }

        /// <summary>
        /// ttl高电平单元list
        /// </summary>
        public ObservableCollection<HighLevelRegion> HighLevelRegions = new ObservableCollection<HighLevelRegion>();

        /// <summary>
        /// 依赖属性
        /// </summary>
        public static readonly DependencyProperty TtlStringProperty = DependencyProperty.Register(
            "TtlString", typeof(string), typeof(TtlSettingWindow),
            new FrameworkPropertyMetadata("0", new PropertyChangedCallback(TtlStringPropertyCallBack)));

        /// <summary>
        /// TTL字符串改变事件
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void TtlStringPropertyCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TtlSettingWindow settingWindow = d as TtlSettingWindow;
            if (settingWindow == null) return;
            char[] chars = settingWindow.TtlString.ToCharArray();
            settingWindow.TtlLengthTextBox.Text = chars.Length.ToString();
            HighLevelRegion region = null;
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == '1')
                {
                    if (region == null)
                    {
                        region = new HighLevelRegion(i, i);
                    }
                    else
                    {
                        region.EndPosition = i;
                    }

                    if (i != chars.Length - 1) continue;
                    settingWindow.HighLevelRegions.Add(region);
                    region = null;
                }
                else
                {
                    if (region == null) continue;
                    settingWindow.HighLevelRegions.Add(region);
                    region = null;
                }
            }
        }

        /// <summary>
        /// 字段TTL string
        /// </summary>
        public string TtlString
        {
            get { return (string) GetValue(TtlStringProperty); }
            set { SetValue(TtlStringProperty, value); }
        }

        /// <summary>
        /// 生成结果字符串
        /// </summary>
        private void GenerateTtlString()
        {
            int listPosition = 0;
            int totalLength = Convert.ToInt32(this.TtlLengthTextBox.Text);
            char[] resultCharArray = new char[totalLength];
            for (var i = 0; i < resultCharArray.Length; i++)
            {
                resultCharArray[i] = '0';
            }

            foreach (var region in this.HighLevelRegions)
            {
                for (int j = region.StartPosition; j <= region.EndPosition; j++)
                {
                    resultCharArray[j] = '1';
                }
            }

            this.ResultTtlString = new string(resultCharArray);
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRegionButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.HighLevelRegions.Remove(this._selectHighLevelRegion);
        }

        /// <summary>
        /// 清空事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteAllRegionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.HighLevelRegions.Clear();
            this.HighRegionsListBox.Items.Refresh();
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateRegionButton_OnClick(object sender, RoutedEventArgs e)
        {
            int start = Convert.ToInt32(this.RegionStartTextBox.Text);
            int end = Convert.ToInt32(this.RegionEndTextBox.Text);
            //            HighLevelRegion selectedRegion = this.HighRegionsListBox.SelectedItem as HighLevelRegion;
            HighLevelRegion selectedRegion = this.HighLevelRegions[this.HighRegionsListBox.SelectedIndex];
            if (selectedRegion == null) return;
            if (!this.VerifyRegion(start, end, selectedRegion))
            {
                MessageBox.Show("高电平起始结束位置冲突", "设置错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            selectedRegion.StartPosition = start;
            selectedRegion.EndPosition = end;
            this.HighRegionsListBox.Items.Refresh();
        }

        /// <summary>
        /// 添加高电平区域事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRegionButton_OnClick(object sender, RoutedEventArgs e)
        {
            int start = Convert.ToInt32(this.RegionStartTextBox.Text);
            int end = Convert.ToInt32(this.RegionEndTextBox.Text);
            if (!this.VerifyRegion(start, end, null))
            {
                MessageBox.Show("高电平起始结束位置冲突", "设置错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            HighLevelRegion newRegion = new HighLevelRegion(start, end);
            this.HighLevelRegions.Insert(this.FindRightPosition4NewRegion(start), newRegion);
        }

        /// <summary>
        /// 确认按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.GenerateTtlString();
            this.Close();
        }

        /// <summary>
        /// 取消按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// 检验region的合法性
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns>是否合法</returns>
        private bool VerifyRegion(int start, int end, HighLevelRegion updateRegion)
        {
            bool legal = true;
            foreach (var region in this.HighLevelRegions)
            {
                if (region == updateRegion)
                {
                    continue;
                }

                if (start >= region.StartPosition - 1 && start <= region.EndPosition + 1)
                {
                    legal = false;
                    break;
                }

                if (end >= region.StartPosition - 1 && end <= region.EndPosition + 1)
                {
                    legal = false;
                    break;
                }
            }

            return legal;
        }

        /// <summary>
        /// 为新region找到合适的位置
        /// </summary>
        /// <param name="start">新region的开始位置</param>
        /// <returns></returns>
        private int FindRightPosition4NewRegion(int start)
        {
            for (int i = 0; i < this.HighLevelRegions.Count; i++)
            {
                if (start < this.HighLevelRegions[i].StartPosition)
                {
                    return i;
                }
            }

            return this.HighLevelRegions.Count;
        }
    }

    /// <summary>
    /// 内部类，标志高电平区域
    /// </summary>
    public class HighLevelRegion
    {
        /// <summary>
        /// 起始位置
        /// </summary>
        private int _startPosition;

        /// <summary>
        /// 起始位置包装器
        /// </summary>
        public int StartPosition
        {
            get { return this._startPosition; }
            set
            {
                if (value > this._endPosition)
                {
                    throw new Exception("开始位置大于结束位置，请重新设置！");
                }

                this._startPosition = value;
            }
        }

        /// <summary>
        /// 结束位置
        /// </summary>
        private int _endPosition;

        /// <summary>
        /// 结束位置包装器
        /// </summary>
        public int EndPosition
        {
            get { return this._endPosition; }
            set
            {
                if (value < this._startPosition)
                {
                    throw new Exception("开始位置大于结束位置，请重新设置！");
                }

                this._endPosition = value;
            }
        }

        /// <summary>
        /// 高电平区域表示字符串
        /// </summary>
//        public string RegionString { get; set; }
        /// <summary>
        /// 高电平区域
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public HighLevelRegion(int start, int end)
        {
            if (start > end)
            {
                throw new Exception("开始位置大于结束位置，请重新设置！");
            }

            this._startPosition = start;
            this._endPosition = end;
        }

        /// <summary>
        /// 构造函数，默认构造函数
        /// </summary>
        public HighLevelRegion() : this(0, 0)
        {
        }

        /// <summary>
        /// 刷新表示字符串方法
        /// </summary>
//        private void RefreshTtlString()
//        {
//            this.RegionString = this._startPosition + " ~ " + this._endPosition;
//        }
        /// <summary>
        /// toString 方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._startPosition + " ~ " + this._endPosition;
        }
    }
}
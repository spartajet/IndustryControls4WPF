using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using IndustryControls4WPF.Annotations;

namespace IndustryControls4WPF.TTL
{
    /// <summary>
    /// NewSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewSettingWindow : Window,INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<TtlStage> _ttlStages;
        public NewSettingWindow(string name, ObservableCollection<TtlStage> ttlStages)
        {
            this._name = name;
            this.TtlStages = ttlStages;
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TtlStage> TtlStages
        {
            get => this._ttlStages;
            set
            {
                this._ttlStages = value;
                this.OnPropertyChanged("TtlStages");
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

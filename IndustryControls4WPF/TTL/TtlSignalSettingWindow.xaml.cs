using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
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
    /// TtlSignalSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TtlSignalSettingWindow : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<TtlSection> _ttlSections;

        public TtlSignalSettingWindow(string name, IEnumerable<TtlSection> ttlSections)
        {
            this.TtlSections = new ObservableCollection<TtlSection>(ttlSections);

            this.InitializeComponent();
            // this.DataGrid.Items.Clear();
            this._name = name;
            this.Title = $"{this._name}Setting";
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TtlSection> TtlSections
        {
            get => this._ttlSections;
            set
            {
                this._ttlSections = value;
                this.OnPropertyChanged("TtlSections");
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.StandardTtlRadioButton.IsChecked == true)
            {
                this.TtlSections.Clear();
            }

            int count = Convert.ToInt32(this.StandardTtlLengthTextBox.Text);
            TtlStatus firstStatus = (TtlStatus) Convert.ToInt32(this.StandardTtlStatusCombobox.SelectedIndex);
            TtlStatus secondStatus = firstStatus == TtlStatus.High ? TtlStatus.Low : TtlStatus.High;
            for (int i = 0; i < count; i++)
            {
                if (i % 2 == 0)
                {
                    this.TtlSections.Add(new TtlSection()
                    {
                        Length = 1,
                        Status = firstStatus
                    });
                }
                else
                {
                    this.TtlSections.Add(new TtlSection()
                    {
                        Length = 1,
                        Status = secondStatus
                    });
                }
            }

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
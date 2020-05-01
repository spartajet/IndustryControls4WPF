using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IndustryControls4WPF.Annotations;
using IndustryControls4WPF.TTL;

namespace IndustryControls4WPF.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        // private ObservableCollection<TtlSection> _ttlSections=Controls.Digital.TtlConfigurator.DefaulTtlSections;
        private ObservableCollection<TtlStage> _ttlStages=new ObservableCollection<TtlStage>()
        {
            new TtlStage()
            {
                Name = "a",
                Repeat = 100,
                TtlSections = new List<TtlSection>(2)
                {
                    new TtlSection()
                    {
                        Length = 0.5,
                        Status = TtlStatus.Low
                    },
                    new TtlSection()
                    {
                        Length = 0.5,
                        Status = TtlStatus.High
                    }
                }
            },
            // new TtlStage()
            // {
            //     Name = "b",
            //     Repeat = 10,
            //     TtlSections = new List<TtlSection>(2)
            //     {
            //         new TtlSection()
            //         {
            //             Length = 20,
            //             Status = TtlStatus.Low
            //         },
            //         new TtlSection()
            //         {
            //             Length = 50,
            //             Status = TtlStatus.High
            //         }
            //     }
            // },
        };
        public MainWindow()
        {
            InitializeComponent();
            // this.TtlConfigurator.TtlSections = this._ttlSections;
            this.TtlConfigurator.TtlStages = this._ttlStages;
        }

        

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // this.TtlConfigurator.TtlString = "0011001010110110";
            // this._ttlSections.Add(new TtlSection()
            // {
            //     Length = 32,
            //     Status = TtlStatus.High
            // });
            // this.TtlConfigurator.TtlSections = this._ttlSections;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

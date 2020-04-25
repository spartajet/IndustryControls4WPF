using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IndustryControls4WPF.TTL
{
    public class TtlStatusConvertor:IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TtlStatus status = (TtlStatus) value;
            switch (status)
            {
                case TtlStatus.High:
                    return "高电平";
                case TtlStatus.Low:
                    return "低电平";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string) value;
            return name.Equals("高电平") ? TtlStatus.High : TtlStatus.Low;
        }
    }
}

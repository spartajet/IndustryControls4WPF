using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryControls4WPF.TTL
{
    public class TtlSection
    {
        private double _length;

        public double Length
        {
            get => this._length;
            set
            {
                // string tempString = (value * 10).ToString("F1");
                int temp = (int) (value * 10);
                if (temp%10==5)
                {
                    this._length = temp / 10.0;
                }
                else
                {
                    this._length = temp / 10;
                }
                // this._length = value;
            }
        }

        public TtlStatus Status { get; set; }
    }
}
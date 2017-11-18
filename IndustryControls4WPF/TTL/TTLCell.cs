using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryControls4WPF.TTL
{
    public class TtlCell
    {
        public int No { get; set; }
        public TtlCellStatus status { get; set; } = TtlCellStatus.Low;

        public TtlCell(int no,TtlCellStatus status)
        {
            this.No = no;
            this.status = status;
        }
    }
}

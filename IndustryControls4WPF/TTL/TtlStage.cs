using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryControls4WPF.TTL
{
    public class TtlStage
    {
        public string Name { get; set; }
        public int Repeat { get; set; }
        public List<TtlSection> TtlSections { get; set; }=new List<TtlSection>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Knob
{
    class ProcessTimer : Timer
    {
        private int pid;
        public ProcessTimer(int pid, double lapse) : base(lapse)
        {
            this.pid = pid;
        }
        public int Pid
        {
            get
            {
                return pid;
            }
        }
    }
}

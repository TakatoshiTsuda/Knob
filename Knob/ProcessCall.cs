using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knob
{
    class ProcessCall
    {
        private string processName;
        private int[] dllInjection;
        private int[] iatHooking;
        private int[] antiDebugging;
        private int[] screenCapture;
        private int dllSeverity;
        private int iatSeverity;
        private int antiDebugSeverity;
        private int screencapSeverity;
        private LinkedList<string> createFileLog;
        private LinkedList<string> writeFileLog;
        private LinkedList<string> deleteFileLog;

        ProcessCall(string processName)
        {
            this.processName = processName;
            dllInjection = new int[4];
            iatHooking = new int[3];
            antiDebugging = new int[3];
            screenCapture = new int[7];
            dllSeverity = 0;
            iatSeverity = 0;
            antiDebugSeverity = 0;
            screencapSeverity = 0;

            createFileLog = new LinkedList<string>();
            writeFileLog = new LinkedList<string>();
            deleteFileLog = new LinkedList<string>();
        }

        public void setFlag(string type)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knob
{
    public class ProcessCall
    {
        string processName;
        int pid;
        int[] dllInjection;
        int[] iatHooking;
        int[] antiDebugging;
        int[] screenCapture;
        int dllSeverity;
        int iatSeverity;
        int antiDebugSeverity;
        int screencapSeverity;
        
        LinkedList<string> log;
        //LinkedList<string> createFileLog;
        //LinkedList<string> writeFileLog;
        //LinkedList<string> deleteFileLog;
        //LinkedList<string> copyFileLog;
        bool alert;

        public ProcessCall(string processName,int pid)
        {
            this.processName = processName;
            this.pid = pid;
            dllInjection = new int[4];
            iatHooking = new int[5];
            antiDebugging = new int[3];
            screenCapture = new int[6];
            dllSeverity = 0;
            iatSeverity = 0;
            antiDebugSeverity = 0;
            screencapSeverity = 0;
            alert = false;
            log = new LinkedList<string>();
            //createFileLog = new LinkedList<string>();
            //writeFileLog = new LinkedList<string>();
            //deleteFileLog = new LinkedList<string>();
            //copyFileLog = new LinkedList<string>();
        }
        public string ProcessName
        {
            get
            {
                return processName;
            }
        }
        public int Pid
        {
            get
            {
                return pid;
            }
        }
        public void setFlag(string type,int number)
        {
            if (type.Equals("dll", StringComparison.OrdinalIgnoreCase))
            {
                if (dllInjection[number] == 0)
                {
                    dllInjection[number] = 1;
                    dllSeverity++;
                }
            }
            else if (type.Equals("iat", StringComparison.OrdinalIgnoreCase))
            {
                if (iatHooking[number] == 0)
                {
                    iatHooking[number] = 1;
                    iatSeverity++;
                }
            }
            else if (type.Equals("anti", StringComparison.OrdinalIgnoreCase))
            {
                if (antiDebugging[number] == 0)
                {
                    antiDebugging[number] = 1;
                    antiDebugSeverity++;
                }
            }
            else if (type.Equals("screencap", StringComparison.OrdinalIgnoreCase))
            {
                if (screenCapture[number] == 0)
                {
                    screenCapture[number] = 1;
                    screencapSeverity++;
                }
            }
            checkSeverity();
        }

        private void checkSeverity()
        {
            if (iatSeverity == 2 || dllSeverity == 2 || screencapSeverity == 2)
            {
                alert = true;
                GlobalManager.procEngine.killProcess(this);
            }
            //else if (antiDebugSeverity >=1 )
            //{
            //    alert = true;
            //}
        }
        public bool Alert
        {
            get;
        }

        public void setLog(string changes)
        {
            log.AddLast(changes);
        }
    }
}

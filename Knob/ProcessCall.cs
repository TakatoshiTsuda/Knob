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
        private bool alert;

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
            alert = false;

            createFileLog = new LinkedList<string>();
            writeFileLog = new LinkedList<string>();
            deleteFileLog = new LinkedList<string>();
        }

        public void setFlag(string type,int number)
        {
            if (type.Equals("dll"))
            {
                if (dllInjection[number] == 0)
                {
                    dllInjection[number] = 1;
                    dllSeverity++;
                }
            }
            else if (type.Equals("iat"))
            {
                if (iatHooking[number] == 0)
                {
                    iatHooking[number] = 1;
                    iatSeverity++;
                }
            }
            else if (type.Equals("anti"))
            {
                if (antiDebugging[number] == 0)
                {
                    antiDebugging[number] = 1;
                    antiDebugSeverity++;
                }
            }
            else
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
            }
            else if (antiDebugSeverity >=1 )
            {
                alert = true;
            }
        }
        public bool Alert
        {
            get
            {
                return Alert;
            }
        }

        public void setLog(string call, string directory)
        {
            if (call.Equals("CreateFile"))
            {
                createFileLog.AddLast(directory);
            }
            else if (call.Equals("DeleteFile"))
            {
                deleteFileLog.AddLast(directory);
            }
            else
            {
                writeFileLog.AddLast(directory);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using Nektra.Deviare2;

namespace Knob
{
    class ProcessEngine
    {
        string sys = "SYSTEM";
        string ls = "LOCAL SERVICE";
        string ns = "NETWORK SERVICE";
        ManagementEventWatcher processStartEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
        ManagementEventWatcher processStopEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
        public ProcessEngine()
        {
            initialize();
            initializeProcessDetector();
        }
        void initialize()
        {
            NktProcessesEnum processes = GlobalManager.spyMgr.Processes();
            NktProcess temp = processes.First();
            while (temp != null)
            {
                filterProcess(1, temp);
                temp = processes.Next();
            }
        }
        void filterProcess(int status, NktProcess proc)
        {
            if (proc.UserName.Equals(sys, StringComparison.OrdinalIgnoreCase) || proc.UserName.Equals(ls, StringComparison.OrdinalIgnoreCase) || proc.UserName.Equals(ns, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Normal Things");
            }
            else
            {
                if (status == 1)
                {
                    GlobalManager.hookEngine.setHook(proc);
                }
                else
                {
                    GlobalManager.removeProc(proc.Name);
                }
                
            }
        }
        NktProcess GetProcess(string proccessName)
        {
            NktProcessesEnum enumProcess = GlobalManager.spyMgr.Processes();
            NktProcess tempProcess = enumProcess.First();
            while (tempProcess != null)
            {
                if (tempProcess.Name.Equals(proccessName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return tempProcess;
                }
                tempProcess = enumProcess.Next();
            }
            return null;
        }
        void initializeProcessDetector()
        {
            processStartEvent.EventArrived += new EventArrivedEventHandler(processStartEvent_EventArrived);
            processStartEvent.Start();
            processStopEvent.EventArrived += new EventArrivedEventHandler(processStopEvent_EventArrived);
            processStopEvent.Start();
        }
        void processStartEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            //string processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();
            NktProcess proc = GetProcess(processName);
            filterProcess(1, proc);            
        }

        void processStopEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            NktProcess proc = GetProcess(processName);
            filterProcess(2, proc);
        }
    }
}

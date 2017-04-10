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
        ManagementEventWatcher processStartEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
        ManagementEventWatcher processStopEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
        ProcessEngine()
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
                filterProcess(temp);
            }
        }
        void filterProcess(NktProcess proc)
        {
            if (!proc.UserName.Equals("SYSTEM") || !proc.UserName.Equals("LOCAL SERVICE") || !proc.UserName.Equals("NETWORK SERVICE"))
            {
                GlobalManager.hookEngine.setHook(proc);
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
            //processStopEvent.EventArrived += new EventArrivedEventHandler(processStopEvent_EventArrived);
            //processStopEvent.Start();
        }
        void processStartEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            //string processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();
            NktProcess proc = GetProcess(processName);
            filterProcess(proc);            
        }

        //void processStopEvent_EventArrived(object sender, EventArrivedEventArgs e)
        //{
        //    string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
        //    string processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();
        //    Console.WriteLine("Process stopped. Name: " + processName + " | ID: " + processID);
        //}
    }
}

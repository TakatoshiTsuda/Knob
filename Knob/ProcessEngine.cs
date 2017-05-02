using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using Nektra.Deviare2;
using System.IO;

namespace Knob
{
    class ProcessEngine
    {
        string sys = "SYSTEM";
        string ls = "LOCAL SERVICE";
        string ns = "NETWORK SERVICE";
        ManagementEventWatcher processStartEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
        ManagementEventWatcher processStopEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
        string dir;
        private string[] exception;
        public ProcessEngine()
        {
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //exception = System.IO.File.ReadAllLines(dir + "/exception.txt");
            initialize();
            initializeProcessDetector();
        }
        void initialize()
        {
            System.IO.StreamWriter file = null;
            int status = 0;
            //Alternate way = When first run listing all the process and put it to exception list
            //To reduce instability and sometimes process take long time to be hooked
            //still thinking a way to add newly run clean program to add to exception list
            //for now just assume that when the program first run, no threat yet
            if (File.Exists(dir + "/exception.txt"))
            {
                exception = System.IO.File.ReadAllLines(dir + "/exception.txt");
                status = 0;
            }
            else
            {
                file = new System.IO.StreamWriter(dir + "/exception.txt", true);
                status = 1;
            }
            NktProcessesEnum processes = GlobalManager.spyMgr.Processes();
            NktProcess temp = processes.First();
            while (temp != null)
            {
                if (status == 0)
                {
                    filterProcess(temp);
                }
                else
                {
                    file.WriteLine(temp.Name);
                }
                temp = processes.Next();
            }
            if (status == 1)
            {
                file.Close();
            }
            exception = System.IO.File.ReadAllLines(dir + "/exception.txt");
            Console.WriteLine("Done Initialize");
        }
        void filterProcess(NktProcess proc)
        {
            if (proc.UserName.Equals(sys, StringComparison.OrdinalIgnoreCase) || proc.UserName.Equals(ls, StringComparison.OrdinalIgnoreCase) || proc.UserName.Equals(ns, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(proc.UserName + " " + proc.Name);
            }
            else
            {
                if (!exception.Contains(proc.Name))
                {
                        GlobalManager.hookEngine.setHook(proc);
                        Console.WriteLine("Hooked " + proc.Name);
                }
                else
                {
                    Console.WriteLine("Exception List" + proc.Name);
                }
                
            }
        }
        public void killProcess(ProcessCall proc)
        {
            Process temp = Process.GetProcessById(proc.Pid);
            temp.Kill();
            Console.WriteLine("Killed a Process -" + proc.ProcessName );
        }
        public void addException(string name)
        {
            System.IO.StreamWriter file = null;
            file = new System.IO.StreamWriter(dir + "/exception.txt", true);
            file.WriteLine(name);
            Console.WriteLine("Added to Exception List! "+ name);
            file.Close();
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
            NktProcess proc = GetProcess(processName);
            if (proc != null)
            {
                Console.WriteLine("Filtering" + proc.Name);
                filterProcess(proc);
            }         
        }
        void processStopEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            int pid = Int32.Parse(e.NewEvent.Properties["ProcessID"].Value.ToString());
            GlobalManager.removeProc(pid);
        }
    }
}

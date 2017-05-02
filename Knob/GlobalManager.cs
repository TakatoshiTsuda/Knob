using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nektra.Deviare2;
using System.Diagnostics;
using System.Timers;

namespace Knob
{
    class GlobalManager
    {
        public static NktSpyMgr spyMgr;
        public static HookEngine hookEngine;
        public static ProcessEngine procEngine;
        private static Dictionary<int, ProcessCall> processList;
        private static Dictionary<int, ProcessTimer> timer;

        public GlobalManager()
        {
            spyMgr = new NktSpyMgr();
            spyMgr.Initialize();
            hookEngine = new HookEngine();
            processList = new Dictionary<int, ProcessCall>();
            timer = new Dictionary<int, ProcessTimer>();
            procEngine = new ProcessEngine();
        }
        public static void addProc(int pid, ProcessCall proc)
        {
            processList.Add(pid, proc);
            Console.WriteLine("Process Added!" + proc.ProcessName);
            ProcessTimer tm = new ProcessTimer(pid, 1000*60*5);
            tm.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            tm.AutoReset = false;
            tm.Enabled = true;
            timer.Add(pid,tm);
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            ProcessTimer temp = (ProcessTimer)sender;
            procEngine.addException(returnProcess(temp.Pid).ProcessName);
        }

        public static void removeProc(int pid)
        {
            ProcessCall pc = returnProcess(pid);
            if ( pc != null)
            {
                processList.Remove(pid);
                Console.WriteLine("Process Removed! " + pc.ProcessName);
            }            
        }
        public static ProcessCall returnProcess(int pid)
        {
            ProcessCall pc;
            processList.TryGetValue(pid, out pc);
            return pc;
        }

    }
}

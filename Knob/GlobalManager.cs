using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nektra.Deviare2;

namespace Knob
{
    class GlobalManager
    {
        public static NktSpyMgr spyMgr;
        public static HookEngine hookEngine;
        private static Dictionary<int, ProcessCall> processList;

        public GlobalManager()
        {
            spyMgr = new NktSpyMgr();
            spyMgr.Initialize();
            hookEngine = new HookEngine();
            processList = new Dictionary<int, ProcessCall>();
        }
        public static void addProc(int pid, ProcessCall proc)
        {
            processList.Add(pid, proc);
            Console.WriteLine("Process Added!" + proc.getName());
        }
        public static void removeProc(int pid)
        {
            ProcessCall pc = returnProcess(pid);
            if ( pc != null)
            {
                processList.Remove(pid);
                Console.WriteLine("Process Removed! " + pc.getName());
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

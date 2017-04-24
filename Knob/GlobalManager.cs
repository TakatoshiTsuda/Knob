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
        private static Dictionary<string, ProcessCall> processList;

        public GlobalManager()
        {
            spyMgr = new NktSpyMgr();
            spyMgr.Initialize();
            hookEngine = new HookEngine();
            processList = new Dictionary<string, ProcessCall>();
        }
        public static void addProc(string name, ProcessCall proc)
        {
            processList.Add(name, proc);
        }
        public static void removeProc(string name)
        {
            processList.Remove(name);
        }
        public static ProcessCall returnProcess(string name)
        {
            ProcessCall pc;
            processList.TryGetValue(name, out pc);
            return pc;
        }

    }
}

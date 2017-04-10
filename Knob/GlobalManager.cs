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

        public GlobalManager()
        {
            spyMgr = new NktSpyMgr();
            spyMgr.Initialize();
            hookEngine = new HookEngine();
        }

    }
}

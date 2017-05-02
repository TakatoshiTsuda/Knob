using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Nektra.Deviare2;
namespace Knob
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            DllRegister dllreg = new DllRegister();
            string dll = (System.IO.Path.GetFullPath("DeviareCOM64.dll"));
            dllreg.Register_Dlls(dll);
            GlobalManager gm = new GlobalManager(); 
            //he.SetHook();
            Boolean temp = true;
            while (temp != false)
            {

            }
        }
    }
}

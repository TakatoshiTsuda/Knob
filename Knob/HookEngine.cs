using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Nektra.Deviare2;

namespace Knob
{
    public class HookEngine
    {
        private NktHooksEnum hooks;
        Dictionary<string,string> calls;
        Dictionary<string,int> callsIdx;
        private string dir;
        public HookEngine()
        {
            hooks = GlobalManager.spyMgr.CreateHooksCollection();
            calls = new Dictionary<string, string>();
            callsIdx = new Dictionary<string, int>();
            initializeCall();

        }
        private void initializeCall()
        {
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] calls = System.IO.File.ReadAllLines(dir + "/systemcall.txt");
            string type = "";
            int idx = 0;
            for (int i = 0; i < calls.Length; i++)
            {
                Console.WriteLine(calls[i]);
                createHook(calls[i]);
                if (i == 0)
                {
                    type = "dll";
                    idx = 0;
                }
                else if (i == 4)
                {
                    type = "iat";
                    idx = 0;
                }
                else if (i == 9)
                {
                    type = "antidebug";
                    idx = 0;
                }
                else if (i == 12)
                {
                    type = "screencap";
                    idx = 0;
                }
                else if (i == 18)
                {
                    type = "logging";
                    idx = 0;
                }
                this.calls.Add(calls[i].Substring(13),type);
                this.callsIdx.Add(calls[i].Substring(13),idx);
                idx++;
            }
            hooks.Hook(true);
        }
        private void createHook(String call)
        {
            NktHook hook = GlobalManager.spyMgr.CreateHook(call, (int)eNktHookFlags.flgOnlyPreCall);
            hook.OnFunctionCalled += OnFunctionCalled;
            hooks.Add(hook);
        }

        public void setHook(NktProcess process)
        {
            hooks.Attach(process, true);
            ProcessCall temp = new ProcessCall(process.Name);
            GlobalManager.addProc(process.Name,temp);
        }

        public void SetHookOld()
        {
            Boolean process = false;
            NktProcess _process = null;
            while (process == false)
            {
                //_process = GetProcess("notepad.exe");
                if (_process != null)
                {
                    process = true;
                    Output("Process Started!\n");
                }
                else
                {
                    Output("Process not started yet\n");
                    System.Threading.Thread.Sleep(5000);

                }
            }
            NktHook hook = GlobalManager.spyMgr.CreateHook("kernel32.dll!CreateFileW", (int)(eNktHookFlags.flgOnlyPreCall));
            NktHooksEnum hooks = GlobalManager.spyMgr.CreateHooksCollection();
            hooks.Add(hook);
            hook.OnFunctionCalled += OnFunctionCalled;
            hook.Hook(true);
            hook.Attach(_process, true);
        }

        void OnFunctionCalled(INktHook hook, INktProcess proc, INktHookCallInfo hookCallInfo)
        {
            string name = proc.Name;
            string hookname = hook.FunctionName;
            ProcessCall pc = GlobalManager.returnProcess(name);
            string type = calls[hookname];
            int idx = callsIdx[hookname];
            pc.setFlag(type,idx);
            

            System.Diagnostics.Debug.WriteLine(name+hookname);
        }
        void OnFunctionCalledOld(INktHook hook, INktProcess proc, INktHookCallInfo hookCallInfo)
        {
            string strCreateFile = "CreateFile(\"";

            INktParamsEnum paramsEnum = hookCallInfo.Params();

            //lpFileName
            INktParam param = paramsEnum.First();
            strCreateFile += param.ReadString() + "\", ";

            //dwDesiredAccess
            param = paramsEnum.Next();
            if ((param.LongVal & 0x80000000) == 0x80000000)
                strCreateFile += "GENERIC_READ ";
            else if ((param.LongVal & 0x40000000) == 0x40000000)
                strCreateFile += "GENERIC_WRITE ";
            else if ((param.LongVal & 0x20000000) == 0x20000000)
                strCreateFile += "GENERIC_EXECUTE ";
            else if ((param.LongVal & 0x10000000) == 0x10000000)
                strCreateFile += "GENERIC_ALL ";
            else
                strCreateFile += "0";
            strCreateFile += ", ";

            //dwShareMode
            param = paramsEnum.Next();
            if ((param.LongVal & 0x00000001) == 0x00000001)
                strCreateFile += "FILE_SHARE_READ ";
            else if ((param.LongVal & 0x00000002) == 0x00000002)
                strCreateFile += "FILE_SHARE_WRITE ";
            else if ((param.LongVal & 0x00000004) == 0x00000004)
                strCreateFile += "FILE_SHARE_DELETE ";
            else
                strCreateFile += "0";
            strCreateFile += ", ";

            //lpSecurityAttributes
            param = paramsEnum.Next();
            if (param.PointerVal != IntPtr.Zero)
            {
                strCreateFile += "SECURITY_ATTRIBUTES(";

                INktParamsEnum paramsEnumStruct = param.Evaluate().Fields();
                INktParam paramStruct = paramsEnumStruct.First();

                strCreateFile += paramStruct.LongVal.ToString();
                strCreateFile += ", ";

                paramStruct = paramsEnumStruct.Next();
                strCreateFile += paramStruct.PointerVal.ToString();
                strCreateFile += ", ";

                paramStruct = paramsEnumStruct.Next();
                strCreateFile += paramStruct.LongVal.ToString();
                strCreateFile += ")";
            }
            else
                strCreateFile += "0";
            strCreateFile += ", ";

            //dwCreationDisposition
            param = paramsEnum.Next();
            if (param.LongVal == 1)
                strCreateFile += "CREATE_NEW ";
            else if (param.LongVal == 2)
                strCreateFile += "CREATE_ALWAYS ";
            else if (param.LongVal == 3)
                strCreateFile += "OPEN_EXISTING ";
            else if (param.LongVal == 4)
                strCreateFile += "OPEN_ALWAYS ";
            else if (param.LongVal == 5)
                strCreateFile += "TRUNCATE_EXISTING ";
            else
                strCreateFile += "0";
            strCreateFile += ", ";

            //dwFlagsAndAttributes
            strCreateFile += param.LongVal;
            strCreateFile += ", ";

            //hTemplateFile
            strCreateFile += param.LongLongVal;
            strCreateFile += ");\r\n";
            Output(strCreateFile);
        }

        void Output(string strCreateFile)
        {
            Console.Write(strCreateFile);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nektra.Deviare2;

namespace Knob
{
    public class HookEngine
    {
        NktSpyMgr _spyMgr = new NktSpyMgr();
        NktHooksEnum hooks;
        public HookEngine()
        {
            _spyMgr.Initialize();
            hooks = _spyMgr.CreateHooksCollection();
        }

        private void createHook(String call)
        {
            NktHook hook = _spyMgr.CreateHook(call,(int)eNktHookFlags.flgOnlyPreCall);
            hooks.Add(hook);
        }
        public void SetHook()
        {
            Boolean process = false;
            NktProcess _process = null;
            while (process == false)
            {
                _process = GetProcess("notepad.exe");
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
            NktHook hook = _spyMgr.CreateHook("kernel32.dll!CreateFileW", (int)(eNktHookFlags.flgOnlyPreCall));
            NktHooksEnum hooks = _spyMgr.CreateHooksCollection();
            hooks.Add(hook);
            hook.OnFunctionCalled += OnFunctionCalled;
            hook.Hook(true);
            hook.Attach(_process, true);
        }
        NktProcess GetProcess(string proccessName)
        {
            NktProcessesEnum enumProcess = _spyMgr.Processes();
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
        void OnFunctionCalled(INktHook hook, INktProcess proc, INktHookCallInfo hookCallInfo)
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

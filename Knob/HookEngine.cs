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
                //this.calls.Add(calls[i].Substring(13),type);
                //callsIdx.Add(calls[i].Substring(13),idx);
                this.calls.Add(calls[i], type);
                callsIdx.Add(calls[i], idx);
                idx++;
            }
            hooks.Hook(true);
        }
        private void createHook(string call)
        {
            NktHook hook = GlobalManager.spyMgr.CreateHook(call, (int)eNktHookFlags.flgOnlyPreCall);
            hook.OnFunctionCalled += OnFunctionCalled;
            hooks.Add(hook);
        }
        public void setHook(NktProcess process)
        {
            hooks.Attach(process, true);
            ProcessCall temp = new ProcessCall(process.Name, process.Id);
            GlobalManager.addProc(process.Id,temp);
            //below used for debugging purposes
            //sometimes hooked succesfully but the call is not detected
            //maybe bug? or suddenly the hook is lost
            //NktHook hk = hooks.First();
            //while (hk != null)
            //{
            //    Console.WriteLine(hk.FunctionName);
            //    hk = hooks.Next();
            //}
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
                    //Output("Process Started!\n");
                }
                else
                {
                    //Output("Process not started yet\n");
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
            ProcessCall pc = GlobalManager.returnProcess(proc.Id);
            string type = calls[hookname];
            int idx = callsIdx[hookname];
            if (type.Equals("logging", StringComparison.OrdinalIgnoreCase))
            {
                string param = getParam(idx, hookCallInfo) ;
                if (param != null)
                {
                    Console.WriteLine(name + " " + param);
                    pc.setLog(param);
                } 
            }
            else
            {
                pc.setFlag(type, idx);
            }
            if (pc.Alert==true)
            {
                Console.WriteLine(name);
            }
            //Console.WriteLine(name + " " + hookname);
        }
        //private NktProcess GetProcess(int pid)
        //{
        //    NktProcessesEnum enumProcess = GlobalManager.spyMgr.Processes();
        //    NktProcess tempProcess = enumProcess.First();
        //    while (tempProcess != null)
        //    {
        //        if (tempProcess.Id == pid)
        //        {
        //            return tempProcess;
        //        }
        //        tempProcess = enumProcess.Next();
        //    }
        //    return null;
        //}
        public string getParam(int idx, INktHookCallInfo info)
        {
            //WriteFile = hFile, lpBuffer, nNumberOfByteToWrite, lpNumberOfBytesWritten,lpOverlapped
            //CreateFile = lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile
            //DeleteFile = lpFileName
            //CopyFile = lpExistingFileName, lpNewFIleName, bFailIfExists
            //CopyFileEx = lpExistingFileName, lpNewFileName, lpProgressROutinem lpData, pbCancel, dwCopyFlags
           
            string parameters = "";
            //idx = 0 = WriteFile
            //idx = 1 = CreateFile
            //idx = 2 = DeleteFile
            //idx = 3 = CopyFile
            //idx = 4 = CopyFileEx
            INktParamsEnum paramEnum = info.Params();
            INktParam param = paramEnum.First();
            
            //while (param != null)
            //{
            //    //still thinking how to handle WriteFile call
            //    //for now, just implements the other beside WriteFile
            //    //the other call didn't have parameters other that WORD or LPCTSTR, so can be processed immediately
            //    //also need to filter for createFile call, so only log truly creating file
            //    parameters += param.ReadString();
            //    param = paramEnum.Next();
            //}
            if (idx == 0) //WriteFile = hFile, lpBuffer, nNumberOfByteToWrite, lpNumberOfBytesWritten,lpOverlapped 
            {
                parameters = "WriteFile - ";
                //for ()
                //{

                //}
                //INktParam param = paramEnum.First();
                //Microsoft.Win32.SafeHandles.SafeFileHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(param.PointerVal,true);
                //handle.
            }
            else if (idx == 1)
            //CreateFile = lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile
            //need to filter whether the call is to CREATEFILE or just to show files in a window
            //both of them using the same call to make it happen
            //still thinking a way to not include the call for showing items in a window as it will hit overall performance
            {
                parameters = "CreateFile - ";
                //param = paramEnum.GetAt(4);
                //if (param.LongVal != 2)
                //{
                //    return null;
                //}
                //lpFileName
                parameters += param.ReadString() + "\", ";

                //dwDesiredAccess
                //seems dwDesiredAccess is the one to filter
                //not this one, so the one to filter is the creationDisposition
                param = paramEnum.Next();
                if ((param.LongVal & 0x80000000) == 0x80000000)
                    parameters += "GENERIC_READ ";
                else if ((param.LongVal & 0x40000000) == 0x40000000)
                    parameters += "GENERIC_WRITE ";
                else if ((param.LongVal & 0x20000000) == 0x20000000)
                    parameters += "GENERIC_EXECUTE ";
                else if ((param.LongVal & 0x10000000) == 0x10000000)
                    parameters += "GENERIC_ALL ";
                else
                    parameters += "0";
                parameters += ", ";

                //dwShareMode
                param = paramEnum.Next();
                if ((param.LongVal & 0x00000001) == 0x00000001)
                    parameters += "FILE_SHARE_READ ";
                else if ((param.LongVal & 0x00000002) == 0x00000002)
                    parameters += "FILE_SHARE_WRITE ";
                else if ((param.LongVal & 0x00000004) == 0x00000004)
                    parameters += "FILE_SHARE_DELETE ";
                else
                    parameters += "0";
                parameters += ", ";

                //lpSecurityAttributes
                param = paramEnum.Next();
                if (param.PointerVal != IntPtr.Zero)
                {
                    parameters += "SECURITY_ATTRIBUTES(";

                    INktParamsEnum paramsEnumStruct = param.Evaluate().Fields();
                    INktParam paramStruct = paramsEnumStruct.First();

                    parameters += paramStruct.LongVal.ToString();
                    parameters += ", ";

                    paramStruct = paramsEnumStruct.Next();
                    parameters += paramStruct.PointerVal.ToString();
                    parameters += ", ";

                    paramStruct = paramsEnumStruct.Next();
                    parameters += paramStruct.LongVal.ToString();
                    parameters += ")";
                }
                else
                    parameters += "0";
                parameters += ", ";

                //dwCreationDisposition
                //seems the flag of creating new file is CREATE_ALWAYS, as other only open, not creating
                //CREATE_NEW only creating file but when the file exists, error shows up
                //other than create_always, will throw null
                param = paramEnum.Next();
                //if (param.LongVal == 1)
                //    parameters += "CREATE_NEW ";
                //else 
                if (param.LongVal == 2)
                    parameters += "CREATE_ALWAYS ";
                //else if (param.LongVal == 3)
                //    parameters += "OPEN_EXISTING ";
                //else if (param.LongVal == 4)
                //    parameters += "OPEN_ALWAYS ";
                //else if (param.LongVal == 5)
                //    parameters += "TRUNCATE_EXISTING ";
                else
                    //parameters += "0";
                    return parameters = null;
                parameters += ", ";

                //dwFlagsAndAttributes
                parameters += param.LongVal;
                parameters += ", ";

                //hTemplateFile
                parameters += param.LongLongVal;
                parameters += ");";
            }
            else if (idx == 2) //DeleteFile = lpFileName
            {
                parameters = "DeleteFile - ";
                parameters += param.ReadString();

            }
            else if (idx == 3 || idx == 4) //CopyFile = lpExistingFileName, lpNewFIleName, bFailIfExists 
            {
                parameters = "CopyFile - ";
                parameters += "Old Folder - " + param.ReadString();
                param = paramEnum.Next();
                parameters += " - New Folder - " + param.ReadString();
            }   
            //else if (idx == 4) //CopyFileEx = lpExistingFileName, lpNewFileName, lpProgressROutinem lpData, pbCancel, dwCopyFlags
            //{

            //}
            return parameters;
        }

        //void OnFunctionCalledOld(INktHook hook, INktProcess proc, INktHookCallInfo hookCallInfo)
        //{
        //    string parameters = "CreateFile(\"";

        //    INktParamsEnum paramsEnum = hookCallInfo.Params();

        //    //lpFileName
        //    INktParam param = paramsEnum.First();
        //    parameters += param.ReadString() + "\", ";

        //    //dwDesiredAccess
        //    param = paramsEnum.Next();
        //    if ((param.LongVal & 0x80000000) == 0x80000000)
        //        parameters += "GENERIC_READ ";
        //    else if ((param.LongVal & 0x40000000) == 0x40000000)
        //        parameters += "GENERIC_WRITE ";
        //    else if ((param.LongVal & 0x20000000) == 0x20000000)
        //        parameters += "GENERIC_EXECUTE ";
        //    else if ((param.LongVal & 0x10000000) == 0x10000000)
        //        parameters += "GENERIC_ALL ";
        //    else
        //        parameters += "0";
        //    parameters += ", ";

        //    //dwShareMode
        //    param = paramsEnum.Next();
        //    if ((param.LongVal & 0x00000001) == 0x00000001)
        //        parameters += "FILE_SHARE_READ ";
        //    else if ((param.LongVal & 0x00000002) == 0x00000002)
        //        parameters += "FILE_SHARE_WRITE ";
        //    else if ((param.LongVal & 0x00000004) == 0x00000004)
        //        parameters += "FILE_SHARE_DELETE ";
        //    else
        //        parameters += "0";
        //    parameters += ", ";

        //    //lpSecurityAttributes
        //    param = paramsEnum.Next();
        //    if (param.PointerVal != IntPtr.Zero)
        //    {
        //        parameters += "SECURITY_ATTRIBUTES(";

        //        INktParamsEnum paramsEnumStruct = param.Evaluate().Fields();
        //        INktParam paramStruct = paramsEnumStruct.First();

        //        parameters += paramStruct.LongVal.ToString();
        //        parameters += ", ";

        //        paramStruct = paramsEnumStruct.Next();
        //        parameters += paramStruct.PointerVal.ToString();
        //        parameters += ", ";

        //        paramStruct = paramsEnumStruct.Next();
        //        parameters += paramStruct.LongVal.ToString();
        //        parameters += ")";
        //    }
        //    else
        //        parameters += "0";
        //    parameters += ", ";

        //    //dwCreationDisposition
        //    param = paramsEnum.Next();
        //    if (param.LongVal == 1)
        //        parameters += "CREATE_NEW ";
        //    else if (param.LongVal == 2)
        //        parameters += "CREATE_ALWAYS ";
        //    else if (param.LongVal == 3)
        //        parameters += "OPEN_EXISTING ";
        //    else if (param.LongVal == 4)
        //        parameters += "OPEN_ALWAYS ";
        //    else if (param.LongVal == 5)
        //        parameters += "TRUNCATE_EXISTING ";
        //    else
        //        parameters += "0";
        //    parameters += ", ";

        //    //dwFlagsAndAttributes
        //    parameters += param.LongVal;
        //    parameters += ", ";

        //    //hTemplateFile
        //    parameters += param.LongLongVal;
        //    parameters += ");\r\n";
        //    //Output(parameters);
        //}

        //void Output(string parameters)
        //{
        //    Console.Write(parameters);
        //}
    }
}

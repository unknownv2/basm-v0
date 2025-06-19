using System;
using System.Diagnostics;

namespace Basm.Memory.Windows
{
    public interface IProcessManager
    {
        Process ProcessHandle { get; }

        int ProcessId { get; }

        IntPtr Search(byte[] searchString);

        IntPtr Search(string searchString);

        long GetMainModuleBaseAddress();

        long GetModuleBaseAddress(string moduleName);

        long GetModuleSize(string moduleName);

        bool Is64BitProcess();

        long AllocMemPtr(int size, long nearAddress);

        bool FreeMemory(long address);

        void Write(long address, byte[] buffer);

        T[] Read<T>(IntPtr address, int offset, int count);

        T Read<T>(IntPtr address, int offset);
    }

}

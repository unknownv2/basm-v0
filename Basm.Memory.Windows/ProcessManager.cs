using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.Memory;
using Binarysharp.MemoryManagement.Native;

namespace Basm.Memory.Windows
{
    public class ProcessManager : IProcessManager
    {
        public Process ProcessHandle { get; }
        public int ProcessId { get; }
        private readonly MemorySharp _memorySharp;
        private List<RemoteAllocation> _allocations;
        public ProcessManager()
        {
            ProcessHandle = Process.GetCurrentProcess();
            ProcessId = ProcessHandle.Id;
            _memorySharp = new MemorySharp(ProcessHandle);
        }

        public ProcessManager(Process process)
        {
            ProcessHandle = process;
            ProcessId = process.Id;
            _memorySharp = new MemorySharp(ProcessHandle);
        }

        public ProcessManager(string processName)
        {
            var apps = ApplicationFinder.FromProcessName(processName);
            if (apps != null && apps.Any())
            {
                ProcessHandle = apps.First();
                ProcessId = ProcessHandle.Id;
                _memorySharp = new MemorySharp(ProcessHandle);
            }
        }

        public IntPtr Search(byte[] searchString)
        {
            throw new NotImplementedException();
        }

        public IntPtr Search(string searchString)
        {
            throw new NotImplementedException();
        }

        public long GetMainModuleBaseAddress()
        {
            return _memorySharp.Modules.MainModule.BaseAddress.ToInt64();
        }

        public long GetModuleBaseAddress(string moduleName)
        {
            return _memorySharp.Modules[moduleName].BaseAddress.ToInt64();
        }

        public long GetModuleSize(string moduleName)
        {
            return _memorySharp.Modules[moduleName].Size;
        }

        public bool Is64BitProcess()
        {
            throw new NotImplementedException();
        }

        public long AllocMemPtr(int size, long nearAddress)
        {
            if (_memorySharp == null)
                throw new Exception("No memory module loaded");

            if(_allocations == null)
                _allocations = new List<RemoteAllocation>();

            var alloc = _memorySharp.Memory.Allocate(size);
            if (alloc != null)
            {
                _allocations.Add(alloc);
                return alloc.BaseAddress.ToInt64();
            }
            return -1;
        }

        public bool FreeMemory(long address)
        {
            if (_allocations.Exists((t => t.BaseAddress.ToInt64() == address)))
            {
                _allocations.Find((t => t.BaseAddress.ToInt64() == address)).Dispose();
            }
            return true;
        }
        public void Write(long address, byte[] buffer)
        {
            if (_memorySharp == null)
                throw new Exception("No memory module loaded");

            _memorySharp[new IntPtr(address), false].Write(buffer);
        }

        public T[] Read<T>(IntPtr address, int offset, int count)
        {
            if (_memorySharp == null)
                throw new Exception("No memory module loaded");

            return _memorySharp[address].Read<T>(offset, count);
        }

        public T Read<T>(IntPtr address, int offset)
        {
            if (_memorySharp == null)
                throw new Exception("No memory module loaded");

            return _memorySharp[address].Read<T>(offset);
        }
    }
}

using System;
using System.IO;
using Basm.Assembler.Directives;
using Basm.Assembler.Implementation;

namespace Basm.Memory.Windows.Processes
{
    public class ProcessMemory : IMemoryAllocator, IMemoryWriter, IMemoryReader
    {
        private readonly IProcessManager _manager;
        public ProcessMemory(IProcessManager manager)
        {
            _manager = manager;
        }
        public long AllocateMemory(int size, long near)
        {
            return _manager.AllocMemPtr(size, near);
        }

        public void FreeMemory(long startAddress)
        {
            if (!_manager.FreeMemory(startAddress))
            {
                throw new Exception("Failed to free memory");
            }
        }

        public void WriteMemory(long address, Stream stream, int length)
        {
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            _manager.Write(address, buffer);
        }

        public byte[] ReadMemory(long address, int length)
        {
            byte[] buffer = _manager.Read<byte>(new IntPtr(address), 0, length);
            if (buffer.Length != length)
                throw new Exception("Failed to read all bytes");
            
            return buffer;
        }
    }
}

using Basm.Assembler.Directives;
using Basm.Assembler.Implementation;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Basm.Memory.Local
{
    public sealed class MarshalMemory : IMemoryAllocator, IMemoryWriter, IMemoryReader
    {
        public long AllocateMemory(int size, long near)
        {
            // TODO: Framework doesn't support specifying a "near" address.
            return Marshal.AllocHGlobal(size).ToInt64();
        }

        public void FreeMemory(long startAddress)
        {
            throw new NotImplementedException();
        }

        public void WriteMemory(long address, Stream stream, int length)
        {
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            Marshal.Copy(buffer, 0, new IntPtr(address), length);
        }

        public byte[] ReadMemory(long address, int length)
        {
            var buffer = new byte[length];

            Marshal.Copy(new IntPtr(address), buffer, 0, length);

            return buffer;
        }
    }
}

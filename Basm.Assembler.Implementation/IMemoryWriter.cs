using System.IO;

namespace Basm.Assembler.Implementation
{
    /// <summary>
    /// Copies streams to memory.
    /// </summary>
    public interface IMemoryWriter
    {
        /// <summary>
        /// Copy a stream to memory.
        /// </summary>
        /// <param name="address">The memory address to copy to.</param>
        /// <param name="stream">The input stream.</param>
        /// <param name="length">The number of bytes to copy.</param>
        void WriteMemory(long address, Stream stream, int length);
    }
}

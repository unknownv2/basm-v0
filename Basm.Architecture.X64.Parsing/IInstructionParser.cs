namespace Basm.Architecture.X64.Parsing
{
    /// <summary>
    /// Parses x86-64 instructions.
    /// </summary>
    public interface IInstructionParser
    {
        /// <summary>
        /// Parse a single instruction into nodes.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <returns>The parsed instruction nodes.</returns>
        IInstructionNode Parse(string instruction);
    }
}

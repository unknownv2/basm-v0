using System.Collections.Generic;

namespace Basm.Assembler
{
    /// <summary>
    /// An assembled script containing sections and local symbols.
    /// </summary>
    public interface IScript
    {
        /// <summary>
        /// The assembler the script was assembled by.
        /// </summary>
        IAssembler Assembler { get; }

        /// <summary>
        /// Script-local symbols.
        /// </summary>
        ISymbolCollection Symbols { get; }

        /// <summary>
        /// Sections defined within the script.
        /// </summary>
        IReadOnlyCollection<ISection> Sections { get; }
    }

    /// <summary>
    /// A named collection of directives and CPU instructions.
    /// </summary>
    public interface ISection
    {
        /// <summary>
        /// The script the section is contained in.
        /// </summary>
        IScript Script { get; }

        /// <summary>
        /// The uppercase name of the section.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execute the directives and instructions
        /// within the section.
        /// </summary>
        void Execute();
    }
}

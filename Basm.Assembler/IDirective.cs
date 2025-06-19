using System;
using System.IO;

namespace Basm.Assembler
{
    /// <summary>
    /// A named command that can be called by assembler scripts.
    /// </summary>
    public interface IDirective
    {
        /// <summary>
        /// The unique name of the directive used by scripts.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execute the directive in the given context.
        /// </summary>
        /// <param name="context">The context the directive is being executed in.</param>
        /// <param name="args">The arguments provided by the script.</param>
        /// <returns>The result value, or null.</returns>
        IConvertible Handle(IScriptContext context, IConvertible[] args);
    }

    /// <summary>
    /// Defines in what context a directive is being
    /// executed in.
    /// </summary>
    public interface IScriptContext
    {
        /// <summary>
        /// The section that the directive is being
        /// executed in.
        /// </summary>
        ISection Section { get; }

        /// <summary>
        /// The instruction stream the directive
        /// appears in, or null if it doesn't.
        /// </summary>
        Stream InstructionStream { get; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace Basm.Assembler
{
    /// <summary>
    /// Assembles scripts and provides extensibility
    /// using custom directives.
    /// </summary>
    public interface IAssembler
    {
        /// <summary>
        /// Symbols accessible by all assembled scripts.
        /// </summary>
        ISymbolCollection Symbols { get; }

        /// <summary>
        /// The directives available to all scripts.
        /// </summary>
        IEnumerable<IDirective> Directives { get; }

        /// <summary>
        /// Register a new directive that can be used in scripts.
        /// </summary>
        /// <param name="directive">The directive handler.</param>
        void RegisterDirective(IDirective directive);

        /// <summary>
        /// Active scripts that have been assembled.
        /// </summary>
        IReadOnlyCollection<IScript> Scripts { get; }

        /// <summary>
        /// Assemble a script.
        /// </summary>
        /// <param name="script">The root node of a script.</param>
        /// <returns>The assembled script.</returns>
        IScript Assemble(IScriptNode script);
    }

    /// <summary>
    /// Base interface of all nodes.
    /// </summary>
    public interface INode
    {

    }

    /// <summary>
    /// The root node of all scripts.
    /// </summary>
    public interface IScriptNode : INode
    {
        /// <summary>
        /// The sections and comments that the 
        /// script contains.
        /// </summary>
        IEnumerable<INode> Children { get; }
    }

    /// <summary>
    /// A named collection of nodes.
    /// </summary>
    public interface ISectionNode : INode
    {
        /// <summary>
        /// The uppercase name of section.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The nodes that the section contains.
        /// </summary>
        IEnumerable<INode> Children { get; }
    }

    /// <summary>
    /// A node that can be computed into a primitive value.
    /// </summary>
    public interface IExpression : INode
    {

    }

    /// <summary>
    /// An expression that applies an operation to two operands.
    /// </summary>
    public interface IBinaryExpression : IExpression
    {
        /// <summary>
        /// The operation to apply to the left and right operands.
        /// </summary>
        BinaryOperator Operator { get; }

        /// <summary>
        /// The operand on the left side of the operator.
        /// </summary>
        IExpression Left { get; }

        /// <summary>
        /// The operand on the right side of the operator.
        /// </summary>
        IExpression Right { get; }
    }

    /// <summary>
    /// An operation that can be applied to two operands.
    /// </summary>
    public enum BinaryOperator
    {
        Multiplication,
        Division,
        Addition,
        Subtraction,
    }

    /// <summary>
    /// An expression that applies an operation to one operand.
    /// </summary>
    public interface IUnaryExpression : IExpression
    {
        /// <summary>
        /// The operation to apply to the operand.
        /// </summary>
        UnaryOperator Operator { get; }

        /// <summary>
        /// The expression the operator will manipulate.
        /// </summary>
        IExpression Operand { get; }
    }

    /// <summary>
    /// An operation that can applied to a single operand.
    /// </summary>
    public enum UnaryOperator
    {
        Positive,
        Negative,
        BitwiseNot,
    }

    /// <summary>
    /// An expression that changes the value type of 
    /// the result of another expression.
    /// </summary>
    public interface ICastExpression : IExpression
    {
        /// <summary>
        /// The type to cast the operand to.
        /// </summary>
        Type CastType { get; }

        /// <summary>
        /// The expression to cast.
        /// </summary>
        IExpression Operand { get; }
    }

    /// <summary>
    /// A call to a named command with the
    /// given arguments.
    /// </summary>
    public interface IDirectiveNode : IExpression
    {
        /// <summary>
        /// The name of the directive.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Directive arguments.
        /// </summary>
        IEnumerable<IExpression> Arguments { get; }
    }

    /// <summary>
    /// Leaf node that represents a native value.
    /// </summary>
    public interface IValueNode : IExpression
    {
        /// <summary>
        /// Get the value as the specific type.
        /// </summary>
        /// <returns>The value.</returns>
        object GetValue(Type type);
    }

    /// <summary>
    /// A node that contains a comment string.
    /// </summary>
    public interface ICommentNode : INode
    {
        /// <summary>
        /// The full comment, possibly spanning multiple lines.
        /// </summary>
        string Comment { get; }
    }

    /// <summary>
    /// Represents a memory address where instructions are
    /// copied to or where execution can be jumped to.
    /// </summary>
    public interface ILabelNode : INode
    {
        /// <summary>
        /// Location or name of the memory address.
        /// </summary>
        IExpression Expression { get; }

        /// <summary>
        /// <see cref="IInstructionNode"/>, <see cref="IDirectiveNode"/>, or <see cref="ICommentNode"/> nodes.
        /// </summary>
        IEnumerable<INode> Children { get; }
    }

    /// <summary>
    /// A single CPU instruction.
    /// </summary>
    public interface IInstructionNode : INode
    {
        /// <summary>
        /// Assemble the instruction into a stream.
        /// </summary>
        /// <param name="stream">A writable stream.</param>
        /// <param name="resolver">An expression resolver.</param>
        /// <returns>Immediate values that need to be patched into the instruction stream.</returns>
        IEnumerable<IUnresolvedImmediateValue> Assemble(Stream stream, IExpressionResolver resolver);

        void SetResolver(IScriptContext context);
        void SetAddress(ulong address);
    }

    /// <summary>
    /// An immediate value that needs to be patched
    /// into the instruction stream at a later time.
    /// </summary>
    public interface IUnresolvedImmediateValue
    {
        /// <summary>
        /// The offset where the immediate value
        /// is located in the instruction stream.
        /// </summary>
        long Offset { get; }

        /// <summary>
        /// True if the patched value should be
        /// a relative address to the beginning
        /// of the instruction.
        /// </summary>
        bool Relative { get; }

        /// <summary>
        /// The size of the immediate value.
        /// </summary>
        ImmediateValueSize Size { get; }

        /// <summary>
        /// The expression to be resolved into 
        /// the immediate value.
        /// </summary>
        IExpression Expression { get; }
    }

    /// <summary>
    /// The byte length of an immediate value.
    /// </summary>
    public enum ImmediateValueSize
    {
        B1 = 1,
        B2 = 2,
        B4 = 4,
        B8 = 8,
    }
}

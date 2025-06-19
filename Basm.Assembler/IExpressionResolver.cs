using System;

namespace Basm.Assembler
{
    /// <summary>
    /// Resolves expressions to primitive values.
    /// </summary>
    public interface IExpressionResolver
    {
        /// <summary>
        /// Resolve an expression to a primitive value.
        /// </summary>
        /// <param name="expresssion">The expression to resolve.</param>
        /// <returns>The result of the expression, or null if resolution should be deferred.</returns>
        object Resolve(IExpression expression, Type type);
    }
}

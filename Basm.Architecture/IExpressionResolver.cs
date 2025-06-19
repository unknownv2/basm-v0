namespace Basm.Architecture
{
    /// <summary>
    /// Resolves arbitrary expressions to immediate values.
    /// </summary>
    public interface IExpressionResolver
    {
        /// <summary>
        /// Resolve an expression into an immediate value.
        /// </summary>
        /// <param name="expression">The expression to resolve.</param>
        /// <returns>The immediate value, or null if resolution failed.</returns>
        object Resolve(object expression);
    }
}

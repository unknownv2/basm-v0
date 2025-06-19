namespace Basm.Architecture.X64.Assemblers.Tests
{
    
    public class UnresolvingExpressionResolver : IExpressionResolver
    {
        public object Resolve(object expression)
        {
            return null;
        }
    }
    
}

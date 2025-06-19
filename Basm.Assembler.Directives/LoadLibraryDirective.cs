using System;

namespace Basm.Assembler.Directives
{
    public sealed class LoadLibraryDirective : IDirective
    {
        private readonly ILibraryLoader _loader;

        public LoadLibraryDirective(ILibraryLoader loader)
        {
            _loader = loader;
        }

        public string Name { get; } = "LoadLibrary";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            return _loader.LoadLibrary(args[0].ToString());
        }
    }

    public interface ILibraryLoader
    {
        long LoadLibrary(string name);
    }
}

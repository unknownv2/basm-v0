using System;
using System.Collections.Generic;

namespace Basm.Assembler
{
    /// <summary>
    /// A collection of symbols. All symbols resolve to
    /// user-mode addresses of up to 64-bits.
    /// </summary>
    public interface ISymbolCollection : IReadOnlyDictionary<string, long>
    {
        /// <summary>
        /// Raised when a symbol is added to the collection.
        /// </summary>
        event EventHandler<SymbolEventArgs> SymbolAdded;

        /// <summary>
        /// Raised when a symbol is removed from the collection.
        /// </summary>
        event EventHandler<SymbolEventArgs> SymbolRemoved;

        /// <summary>
        /// Raised when a symbol value is changed.
        /// </summary>
        event EventHandler<SymbolEventArgs> SymbolChanged;

        /// <summary>
        /// Add, update, or get a symbol value from the collection.
        /// </summary>
        /// <param name="name">The name of the symbol.</param>
        /// <returns>The symbol value.</returns>
        new long this[string name] { get; set; }

        /// <summary>
        /// Add a symbol to the collection.
        /// </summary>
        /// <param name="name">The name of the symbol.</param>
        /// <param name="value">The value of the symbol.</param>
        void Add(string name, long value);

        /// <summary>
        /// Remove a symbol from the collection.
        /// </summary>
        /// <param name="name">The name of the symbol.</param>
        /// <returns>True if a symbol with the given name was removed.</returns>
        bool Remove(string name);
    }

    /// <summary>
    /// Event arguments for symbols.
    /// </summary>
    public sealed class SymbolEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the symbol that was changed.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The new value of the symbol, or the value
        /// of the symbol before it was removed.
        /// </summary>
        public long Value { get; }

        public SymbolEventArgs(string name, long value)
        {
            Name = name;
            Value = value;
        }
    }
}

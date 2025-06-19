using System;
using System.Collections;
using System.Collections.Generic;

namespace Basm.Assembler.Implementation
{
    internal sealed class SymbolCollection : ISymbolCollection
    {
        private readonly Dictionary<string, long> _symbols;

        public SymbolCollection(int initialCapacity)
        {
            _symbols = new Dictionary<string, long>(initialCapacity);
        }

        public long this[string name]
        {
            get
            {
                lock (_symbols)
                {
                    return _symbols[name];
                }
            }
            set
            {
                bool existed;
                lock (_symbols)
                {
                    existed = _symbols.TryGetValue(name, out long currentValue);
                    _symbols[name] = value;
                }

                var e = new SymbolEventArgs(name, value);
                if (existed)
                {
                    SymbolChanged?.Invoke(this, e);
                }
                else
                {
                    SymbolAdded?.Invoke(this, e);
                }
            }
        }

        long IReadOnlyDictionary<string, long>.this[string key] => this[key];
        public IEnumerable<string> Keys => _symbols.Keys;
        public IEnumerable<long> Values => _symbols.Values;
        public int Count => _symbols.Count;

        public event EventHandler<SymbolEventArgs> SymbolAdded;
        public event EventHandler<SymbolEventArgs> SymbolRemoved;
        public event EventHandler<SymbolEventArgs> SymbolChanged;

        public void Add(string name, long value)
        {
            lock (_symbols)
            {
                _symbols.Add(name, value);
            }

            SymbolAdded?.Invoke(this, new SymbolEventArgs(name, value));
        }

        public bool ContainsKey(string key)
        {
            lock (_symbols)
            {
                return _symbols.ContainsKey(key);
            }
        }

        public IEnumerator<KeyValuePair<string, long>> GetEnumerator()
        {
            lock (_symbols)
            {
                return _symbols.GetEnumerator();
            }
        }

        public bool Remove(string name)
        {
            bool removed;
            long value;
            lock (_symbols)
            {
                _symbols.TryGetValue(name, out value);
                removed = _symbols.Remove(name);
            }
            
            if (removed)
            {
                SymbolRemoved?.Invoke(this, new SymbolEventArgs(name, value));
            }

            return removed;
        }

        public bool TryGetValue(string key, out long value)
        {
            lock (_symbols)
            {
                return _symbols.TryGetValue(key, out value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_symbols)
            {
                return ((IEnumerable)_symbols).GetEnumerator();
            }
        }
    }
}

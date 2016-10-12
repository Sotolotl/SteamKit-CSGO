using System.Collections.Generic;

namespace CSGO
{
    /// <inheritdoc />
    internal class SingleUseDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new TValue this[TKey key]
        {
            get
            {
                TValue val = base[key];
                Remove(key);
                return val;
            }
            set { base[key] = value; }
        }
    }
}

using System.Collections.Generic;

namespace CSGO
{
    /// <summary>
    /// Single Use Dictionary, removes an entry after it has been read.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
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

        public new bool TryGetValue(TKey key, out TValue value)
        {
            if (this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

    }
}

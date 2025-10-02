using System.Collections.Concurrent;

namespace BrightSword.SwissKnife
{
    public class ConcurrentDictionary<TKey1, TKey2, TValue> :
      ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>
    {
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get => this[key1][key2];
            set
            {
                var inner = GetOrAdd(key1, new ConcurrentDictionary<TKey2, TValue>());
                inner[key2] = value;
            }
        }
    }
}

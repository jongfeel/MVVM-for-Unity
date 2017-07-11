using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Data
{
    public class PropertyNameValuePair<TKey, TValue>
    {
        public PropertyNameValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { set;  get; }
        public TValue Value { set;  get; }
    }
}

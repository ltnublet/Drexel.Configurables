using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    public sealed class Configuration : IConfiguration
    {
        public object this[IRequirement key] => throw new NotImplementedException();

        public IEnumerable<IRequirement> Keys => throw new NotImplementedException();

        public IEnumerable<object> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool ContainsKey(IRequirement key)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<IRequirement, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(IRequirement key, out object value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

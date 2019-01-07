using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Drexel.Configurables.Example.Contracts;

namespace Drexel.Configurables.Example
{
    public class ApplicationContext : IApplicationContext
    {
        private readonly Dictionary<string, IMenuBinding> bindings;

        public ApplicationContext()
        {
            this.bindings = new Dictionary<string, IMenuBinding>();
            this.Bindings = new ReadOnlyDictionary<string, IMenuBinding>(this.bindings);
        }

        internal event EventHandler<IExamplePlugin?> ShutdownRequested;

        public IReadOnlyDictionary<string, IMenuBinding> Bindings { get; private set; }

        IEnumerable<IMenuBinding> IApplicationContext.Bindings => this.Bindings.Values;

        public void AddMenuBinding(IMenuBinding binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            if (this.bindings.ContainsKey(binding.Token))
            {
                throw new ArgumentException("A binding with the specified token already exists.", nameof(binding));
            }

            this.bindings.Add(binding.Token, binding);
            this.Bindings = new ReadOnlyDictionary<string, IMenuBinding>(this.bindings);

        }

        public bool RequestShutdown(IMenuBinding requestor)
        {
            if (requestor == null)
            {
                throw new ArgumentNullException(nameof(requestor));
            }

            if (requestor.AssociatedPlugin.GetType() != typeof(SystemPlugin))
            {
                return false;
            }

            this.ShutDown(requestor.AssociatedPlugin);
            return true;
        }

        internal void ShutDown(IExamplePlugin? requestor = null)
        {
            this.ShutdownRequested.Invoke(this, requestor);
        }
    }
}

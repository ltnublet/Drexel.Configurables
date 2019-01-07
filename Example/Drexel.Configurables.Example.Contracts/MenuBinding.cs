using System;
using System.Threading.Tasks;

namespace Drexel.Configurables.Example.Contracts
{
    public class MenuBinding : IMenuBinding
    {
        private readonly Func<IMenuBinding, ConsoleInstance, Task> callback;

        public MenuBinding(
            string token,
            string description,
            IExamplePlugin associatedPlugin,
            Func<IMenuBinding, ConsoleInstance, Task> callback)
        {
            this.Token = token ?? throw new ArgumentNullException(nameof(token));
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.AssociatedPlugin = associatedPlugin ?? throw new ArgumentNullException(nameof(associatedPlugin));
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public IExamplePlugin AssociatedPlugin { get; }

        public string Description { get; }

        public string Token { get; }

        public Task Callback(IMenuBinding self, ConsoleInstance console) => this.callback.Invoke(self, console);
    }
}

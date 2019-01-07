using System.Collections.Generic;

namespace Drexel.Configurables.Example.Contracts
{
    public interface IApplicationContext
    {
        IEnumerable<IMenuBinding> Bindings { get; }

        void AddMenuBinding(IMenuBinding binding);

        bool RequestShutdown(IMenuBinding requestor);
    }
}

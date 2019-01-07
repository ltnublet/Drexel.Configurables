using System.Threading.Tasks;

namespace Drexel.Configurables.Example.Contracts
{
    public interface IMenuBinding
    {
        IExamplePlugin AssociatedPlugin { get; }
        
        string Description { get; }

        string Token { get; }

        Task Callback(IMenuBinding self, ConsoleInstance console);
    }
}

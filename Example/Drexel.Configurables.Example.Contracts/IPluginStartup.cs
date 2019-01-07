using System.Threading.Tasks;

namespace Drexel.Configurables.Example.Contracts
{
    public interface IPluginStartup
    {
        Task OnStartup(IApplicationContext context);

        Task OnShutdown();
    }
}

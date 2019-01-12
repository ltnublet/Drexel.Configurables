namespace Drexel.Configurables.Serialization
{
    public interface IStructTypeSerializer<T> : ITypeSerializer
        where T : struct
    {
    }
}

using Fathcore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Infrastructures.Abstractions
{
    /// <summary>
    /// Represents dependency registrar
    /// </summary>
    public interface IDependencyRegistrar : ISingletonService
    {
    }
}

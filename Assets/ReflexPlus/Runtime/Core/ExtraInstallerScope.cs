using System;
using ReflexPlus.Injectors;

namespace ReflexPlus.Core
{
    public class ExtraInstallerScope : IDisposable
    {
        private readonly Action<ContainerBuilder> extraInstaller;

        public ExtraInstallerScope(Action<ContainerBuilder> extraInstaller)
        {
            this.extraInstaller = extraInstaller;
            UnityInjector.ExtraInstallers += this.extraInstaller;
        }

        public void Dispose()
        {
            UnityInjector.ExtraInstallers -= extraInstaller;
        }
    }
}
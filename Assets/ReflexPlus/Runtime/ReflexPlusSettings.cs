using ReflexPlus.Core;
using ReflexPlus.Logging;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReflexPlus
{
    internal sealed class ReflexPlusSettings : ScriptableObject
    {
        private const string FileName = nameof(ReflexPlusSettings);

        private static ReflexPlusSettings instance;

        public static ReflexPlusSettings Instance
        {
            get
            {
                if (!instance)
                    instance = Resources.Load<ReflexPlusSettings>(FileName);

                Assert.IsNotNull(instance,
                    $"{FileName} not found in Resources folder.\n"
                    + $"Please create {FileName} using right mouse button over Resources folder, Create > Reflex+ > Settings.");
                return instance;
            }
        }

        [field: SerializeField]
        public LogLevel LogLevel { get; private set; }

        [field: SerializeField]
        public ContainerScope RootScope { get; private set; }

        private void OnValidate()
        {
            instance = this;
            ReflexPlusLogger.UpdateLogLevel(LogLevel);
        }
    }
}
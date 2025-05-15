using System.Reflection;
using ReflexPlus.Core;
using ReflexPlus.Injectors;

namespace ReflexPlus.Injectables
{
    public readonly struct InjectableFieldInfo
    {
        public InjectableFieldInfo(RegistrationId registrationId, FieldInfo fieldInfo, bool optional)
        {
            RegistrationId = registrationId;
            FieldInfo = fieldInfo;
            Optional = optional;
        }

        public RegistrationId RegistrationId { get; }

        public FieldInfo FieldInfo { get; }

        public bool Optional { get; }

        public void InjectInto(object obj, Container container)
        {
            FieldInjector.Inject(FieldInfo, Optional, RegistrationId.Key, obj, container);
        }
    }
}
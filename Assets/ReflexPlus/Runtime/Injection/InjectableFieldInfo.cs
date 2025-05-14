using System.Reflection;
using Reflex.Core;
using Reflex.Injectors;
using Reflex.Registration;

namespace Reflex.Injection
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

        public void InjectInto(object obj, object key, Container container)
        {
            var targetRegistrationId = new RegistrationId(FieldInfo.FieldType, key);
            if (!RegistrationId.Equals(targetRegistrationId))
                return;
            
            FieldInjector.Inject(FieldInfo, obj, container);
        }
    }
}
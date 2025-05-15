using System.Reflection;

namespace ReflexPlus.Injectables
{
    public readonly struct InjectablePropertyInfo
    {
        public InjectablePropertyInfo(RegistrationId registrationId, PropertyInfo propertyInfo, bool optional)
        {
            RegistrationId = registrationId;
            PropertyInfo = propertyInfo;
            Optional = optional;
        }

        public RegistrationId RegistrationId { get; }

        public PropertyInfo PropertyInfo { get; }

        public bool Optional { get; }
    }
}
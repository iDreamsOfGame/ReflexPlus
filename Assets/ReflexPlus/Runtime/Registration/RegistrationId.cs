using System;

namespace Reflex.Registration
{
    public readonly struct RegistrationId : IEquatable<RegistrationId>
    {
        public RegistrationId(Type type, object key = null)
        {
            Type = type;
            Key = key;
        }

        public Type Type { get; }

        public object Key { get; }

        public bool Equals(RegistrationId other) => other.Type == Type && other.Key == Key;

        public override bool Equals(object obj) => obj is RegistrationId other && Equals(other);

        public override int GetHashCode() => Key != null ? HashCode.Combine(Type, Key) : Type.GetHashCode();
    }
}
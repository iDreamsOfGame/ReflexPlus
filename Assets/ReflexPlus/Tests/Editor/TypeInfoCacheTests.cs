using System.Linq;
using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Caching;

namespace ReflexPlusEditor.Tests
{
    internal class TypeInfoCacheTests
    {
        private class HasPublicField
        {
            [Inject]
            public int Number;
        }

        private class HasPublicProperty
        {
            [Inject]
            public int Number { get; private set; }
        }

        private class HasPublicMethod
        {
            [Inject]
            public void Inject(int number)
            {
            }
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicFields()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPublicField));
            Assert.That(typeInfo.InjectableFields.Single().FieldInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicProperties()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPublicProperty));
            Assert.That(typeInfo.InjectableProperties.Single().PropertyInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicMethods()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPublicMethod));
            Assert.That(typeInfo.InjectableMethods.Single().MethodInfo.Name, Is.EqualTo("Inject"));
        }
        
        private class HasProtectedField
        {
            [Inject]
            protected int Number;
        }

        private class HasProtectedProperty
        {
            [Inject]
            protected int Number { get; private set; }
        }

        private class HasProtectedMethod
        {
            [Inject]
            protected void Inject(int number)
            {
            }
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedFields()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasProtectedField));
            Assert.That(typeInfo.InjectableFields.Single().FieldInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedProperties()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasProtectedProperty));
            Assert.That(typeInfo.InjectableProperties.Single().PropertyInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedMethods()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasProtectedMethod));
            Assert.That(typeInfo.InjectableMethods.Single().MethodInfo.Name, Is.EqualTo("Inject"));
        }

        private class HasPrivateField
        {
            [Inject]
            private int number;
        }

        private class HasPrivateProperty
        {
            [Inject]
            private int Number { get; set; }
        }

        private class HasPrivateMethod
        {
            [Inject]
            private void Inject(int number)
            {
            }
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateFields()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPrivateField));
            Assert.That(typeInfo.InjectableFields.Single().FieldInfo.Name, Is.EqualTo("number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateProperties()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPrivateProperty));
            Assert.That(typeInfo.InjectableProperties.Single().PropertyInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateMethods()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPrivateMethod));
            Assert.That(typeInfo.InjectableMethods.Single().MethodInfo.Name, Is.EqualTo("Inject"));
        }

        private class HasParentWithPublicField : HasPublicField
        {
        }

        private class HasParentWithPublicProperty : HasPublicProperty
        {
        }

        private class HasParentWithPublicMethod : HasPublicMethod
        {
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicFields_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPublicField));
            Assert.That(typeInfo.InjectableFields.Single().FieldInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicProperties_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPublicProperty));
            Assert.That(typeInfo.InjectableProperties.Single().PropertyInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicMethods_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPublicMethod));
            Assert.That(typeInfo.InjectableMethods.Single().MethodInfo.Name, Is.EqualTo("Inject"));
        }

        private class HasParentWithProtectedField : HasProtectedField
        {
        }

        private class HasParentWithProtectedProperty : HasProtectedProperty
        {
        }

        private class HasParentWithProtectedMethod : HasProtectedMethod
        {
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedFields_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithProtectedField));
            Assert.That(typeInfo.InjectableFields.Single().FieldInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedProperties_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithProtectedProperty));
            Assert.That(typeInfo.InjectableProperties.Single().PropertyInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedMethods_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithProtectedMethod));
            Assert.That(typeInfo.InjectableMethods.Single().MethodInfo.Name, Is.EqualTo("Inject"));
        }

        private class HasParentWithPrivateField : HasPrivateField
        {
        }

        private class HasParentWithPrivateProperty : HasPrivateProperty
        {
        }

        private class HasParentWithPrivateMethod : HasPrivateMethod
        {
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateFields_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPrivateField));
            Assert.That(typeInfo.InjectableFields.Single().FieldInfo.Name, Is.EqualTo("number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateProperties_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPrivateProperty));
            Assert.That(typeInfo.InjectableProperties.Single().PropertyInfo.Name, Is.EqualTo("Number"));
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateMethods_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPrivateMethod));
            Assert.That(typeInfo.InjectableMethods.Single().MethodInfo.Name, Is.EqualTo("Inject"));
        }
    }
}
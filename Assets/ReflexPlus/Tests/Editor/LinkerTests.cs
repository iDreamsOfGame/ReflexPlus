using NUnit.Framework;
using UnityEngine;

namespace ReflexPlusEditor.Tests
{
    internal class LinkerTests
    {
        [Test]
        public void LinkerFileExist_ReturnsNotNull()
        {
            var linker = Resources.Load<TextAsset>("link");
            Assert.That(linker, Is.Not.Null);
        }
    }
}
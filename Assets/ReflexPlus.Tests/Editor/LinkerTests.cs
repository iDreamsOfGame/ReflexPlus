using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace ReflexPlus.EditModeTests
{
    public class LinkerTests
    {
        [Test]
        public void LinkerFileExist()
        {
            var linker = Resources.Load<TextAsset>("link");
            linker.Should().NotBeNull();
        }
    }
}
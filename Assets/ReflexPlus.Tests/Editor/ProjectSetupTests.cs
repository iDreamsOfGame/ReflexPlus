using NUnit.Framework;

namespace ReflexPlus.EditModeTests
{
    public class ProjectSetupTests
    {
        [Test]
        public void ProjectShouldHaveReflexSettings()
        {
            if (ReflexPlusSettings.Instance)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"Project is missing ReflexSettings scriptable object at Resources folder.");
            }
        }
    }
}
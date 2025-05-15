using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

// https://github.com/jilleJr/Newtonsoft.Json-for-Unity/wiki/Embed-link.xml-in-UPM-package
namespace ReflexPlusEditor.Linking
{
    internal sealed class Linker : IUnityLinkerProcessor
    {
        public int callbackOrder { get; }

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            return Path.GetFullPath("Packages/io.github.idreamsofgame.reflexplus/Resources/link.xml");
        }
    }
}
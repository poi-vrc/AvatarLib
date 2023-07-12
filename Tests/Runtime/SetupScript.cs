using NUnit.Framework;
using UnityEditor;
using UnityEditor.PackageManager.UI;

namespace Chocopoi.AvatarLib.Animations.Tests
{
    [SetUpFixture]
    public class SetupScript
    {
        public const string SampleFolder = "Assets/Samples/AvatarLib/1.0.0/Samples for Unit Testing";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // Load the unit testing sample to project

            bool imported = false;

            foreach (Sample sample in Sample.FindByPackage("com.chocopoi.vrc.avatarlib", "1.0.0"))
            {
                if (sample.displayName == "Samples for Unit Testing")
                {
                    sample.Import(Sample.ImportOptions.OverridePreviousImports);
                    imported = true;
                    break;
                }
            }

            if (!imported)
            {
                throw new System.Exception("AvatarLib could not load the sample for unit testing!");
            }
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
#if AVATARLIB_TEST_DEBUG
            AssetDatabase.DeleteAsset(SampleFolder);
#endif
        }
    }
}

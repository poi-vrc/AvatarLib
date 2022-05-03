using System.Collections;
using System.Collections.Generic;
using Chocopoi.AvatarLib.Animations;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace Chocopoi.AvatarLib.Animations.Tests
{
    public class AnimationUtilsTestScript
    {
        private GameObject prefab1 = AssetDatabase.LoadAssetAtPath<GameObject>(SetupScript.SampleFolder + "/Prefabs/AnimationUtilsTestObject1.prefab");

        private GameObject prefab2 = AssetDatabase.LoadAssetAtPath<GameObject>(SetupScript.SampleFolder + "/Prefabs/AnimationUtilsTestObject2.prefab");

        private static void MakeDebugAnimAsset(AnimationClip clip, string testName)
        {
#if !AVATARLIB_TEST_DEBUG
            if (!AssetDatabase.IsValidFolder(SetupScript.SampleFolder))
            {
                Debug.LogError("Could not create debug anim because the sample folder does not exist: " + testName);
            }
            AssetDatabase.CreateAsset(clip, SetupScript.SampleFolder + "/Test_Debug_" + testName + ".anim");
#endif
        }

        //
        // GetRelativePath
        //

        [Test]
        public void GetRelativePath_ShouldReturnValidPath_WithoutUntilTransform()
        {
            Transform obj2 = prefab1.transform.Find("Object2");
            Transform obj3 = obj2.transform.Find("Object3");
            Transform obj6 = obj3.transform.Find("Object6");
            Transform obj7 = obj6.transform.Find("Object7");

            string path = AnimationUtils.GetRelativePath(obj7);
            Assert.AreEqual("Object2/Object3/Object6/Object7", path);
        }

        [Test]
        public void GetRelativePath_ShouldReturnValidPath_WithUntilTransform()
        {
            Transform obj2 = prefab1.transform.Find("Object2");
            Transform obj3 = obj2.transform.Find("Object3");
            Transform obj6 = obj3.transform.Find("Object6");
            Transform obj7 = obj6.transform.Find("Object7");

            string path = AnimationUtils.GetRelativePath(obj7, obj3);
            Assert.AreEqual("Object6/Object7", path);
        }

        [Test]
        public void GetRelativePath_ShouldReturnValidPath_WithPrefixSuffix()
        {
            Transform obj2 = prefab1.transform.Find("Object2");
            Transform obj3 = obj2.transform.Find("Object3");
            Transform obj6 = obj3.transform.Find("Object6");

            string path = AnimationUtils.GetRelativePath(obj6, null, "AnimationUtilsTestObject1/", "/Object7");
            Assert.AreEqual("AnimationUtilsTestObject1/Object2/Object3/Object6/Object7", path);
        }

        //
        // SetSingleFrameGameObjectEnabledCurves
        //

        // assert whether the first frame matches the expected value
        private static void AssertValidIsActiveSingleFrameCurve(AnimationClip clip, string path, string propertyName, float expectedValue)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, new EditorCurveBinding()
            {
                path = path,
                propertyName = propertyName,
                type = typeof(GameObject)
            });

            Assert.NotNull(curve);
            Assert.AreEqual(1, curve.length);

            Keyframe keyFrame = curve.keys[0];
            Assert.AreEqual(expectedValue, keyFrame.value);
        }

        [Test]
        public void SetSingleFrameGameObjectEnabledCurves_ShouldGenerateSingleFrameTrueCurve()
        {
            AnimationClip clip = new AnimationClip();

            GameObject obj2 = prefab1.transform.Find("Object2").gameObject;
            GameObject obj3 = obj2.transform.Find("Object3").gameObject;
            GameObject obj6 = obj3.transform.Find("Object6").gameObject;
            GameObject obj7 = obj6.transform.Find("Object7").gameObject;

            AnimationUtils.SetSingleFrameGameObjectEnabledCurves(clip, new GameObject[] { obj2, obj3, obj6, obj7 }, true);

            //For debug purposes
            MakeDebugAnimAsset(clip, "SetSingleFrameGameObjectEnabledCurves_ShouldGenerateSingleFrameTrueCurve");

            Assert.AreEqual(0.0f, clip.length);

            AssertValidIsActiveSingleFrameCurve(clip, "Object2", "m_IsActive", 1.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object2/Object3", "m_IsActive", 1.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object2/Object3/Object6", "m_IsActive", 1.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object2/Object3/Object6/Object7", "m_IsActive", 1.0f);
        }

        [Test]
        public void SetSingleFrameGameObjectEnabledCurves_ShouldGenerateSingleFrameFalseCurve()
        {
            AnimationClip clip = new AnimationClip();

            GameObject obj2 = prefab1.transform.Find("Object2").gameObject;
            GameObject obj3 = obj2.transform.Find("Object3").gameObject;
            GameObject obj6 = obj3.transform.Find("Object6").gameObject;
            GameObject obj7 = obj6.transform.Find("Object7").gameObject;

            AnimationUtils.SetSingleFrameGameObjectEnabledCurves(clip, new GameObject[] { obj2, obj3, obj6, obj7 }, false);

            //For debug purposes
            MakeDebugAnimAsset(clip, "SetSingleFrameGameObjectEnabledCurves_ShouldGenerateSingleFrameFalseCurve");

            Assert.AreEqual(0.0f, clip.length);

            AssertValidIsActiveSingleFrameCurve(clip, "Object2", "m_IsActive", 0.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object2/Object3", "m_IsActive", 0.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object2/Object3/Object6", "m_IsActive", 0.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object2/Object3/Object6/Object7", "m_IsActive", 0.0f);
        }

        //
        // SetSingleFrameComponentEnabledCurves
        //

        [Test]
        public void SetSingleFrameComponentEnabledCurves_ShouldGenerateSingleFrameTrueCurve()
        {
            AnimationClip clip = new AnimationClip();

            VRCPhysBone[] comps = prefab2.GetComponentsInChildren<VRCPhysBone>();

            AnimationUtils.SetSingleFrameComponentEnabledCurves(clip, comps, true);

            //For debug purposes
            MakeDebugAnimAsset(clip, "SetSingleFrameComponentEnabledCurves_ShouldGenerateSingleFrameTrueCurve");

            Assert.AreEqual(0.0f, clip.length);

            AssertValidIsActiveSingleFrameCurve(clip, "Object1/PhysBone1", "m_Enabled", 1.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object1/PhysBone2", "m_Enabled", 1.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object1/Object2/PhysBone3", "m_Enabled", 1.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object3/PhysBone4", "m_Enabled", 1.0f);
        }

        [Test]
        public void SetSingleFrameComponentEnabledCurves_ShouldGenerateSingleFrameFalseCurve()
        {
            AnimationClip clip = new AnimationClip();

            VRCPhysBone[] comps = prefab2.GetComponentsInChildren<VRCPhysBone>();

            AnimationUtils.SetSingleFrameComponentEnabledCurves(clip, comps, false);

            //For debug purposes
            MakeDebugAnimAsset(clip, "SetSingleFrameComponentEnabledCurves_ShouldGenerateSingleFrameFalseCurve");

            Assert.AreEqual(0.0f, clip.length);

            AssertValidIsActiveSingleFrameCurve(clip, "Object1/PhysBone1", "m_Enabled", 0.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object1/PhysBone2", "m_Enabled", 0.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object1/Object2/PhysBone3", "m_Enabled", 0.0f);
            AssertValidIsActiveSingleFrameCurve(clip, "Object3/PhysBone4", "m_Enabled", 0.0f);
        }

        [Test]
        public void SetComponentEnabledCurves_ShouldGenerateThreeFrameCurve()
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = 30.0f;

            VRCPhysBone[] comps = prefab2.GetComponentsInChildren<VRCPhysBone>();

            AnimationUtils.SetComponentEnabledCurves(clip, comps, GenerateDummyThreeFrameCurve());

            //For debug purposes
            MakeDebugAnimAsset(clip, "SetComponentEnabledCurves_ShouldGenerateThreeFrameCurve");

            Assert.AreEqual(2.0f / 30.0f, clip.length);

            AssertValidIsActiveThreeFrameCurve(clip, "Object1/PhysBone1", "m_Enabled", typeof(VRCPhysBone));
            AssertValidIsActiveThreeFrameCurve(clip, "Object1/PhysBone2", "m_Enabled", typeof(VRCPhysBone));
            AssertValidIsActiveThreeFrameCurve(clip, "Object1/Object2/PhysBone3", "m_Enabled", typeof(VRCPhysBone));
            AssertValidIsActiveThreeFrameCurve(clip, "Object3/PhysBone4", "m_Enabled", typeof(VRCPhysBone));
        }

        //
        // SetGameObjectEnabledCurves
        //

        // assert whether the three frames are 0.0f,1.0f,0.0f
        private static void AssertValidIsActiveThreeFrameCurve(AnimationClip clip, string path, string propertyName, System.Type type)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, new EditorCurveBinding()
            {
                path = path,
                propertyName = propertyName,
                type = type
            });

            Assert.NotNull(curve);
            Assert.AreEqual(3, curve.length);

            Assert.AreEqual(0.0f, curve.keys[0].value);
            Assert.AreEqual(1.0f, curve.keys[1].value);
            Assert.AreEqual(0.0f, curve.keys[2].value);
        }

        // creates a dummy curve for testing
        private static AnimationCurve GenerateDummyThreeFrameCurve()
        {
            AnimationCurve c = AnimationCurve.Constant(0.0f, 0.0f, 0.0f);
            c.AddKey(1.0f / 30.0f, 1.0f);
            c.AddKey(2.0f / 30.0f, 0.0f);
            return c;
        }

        [Test]
        public void SetGameObjectEnabledCurves_ShouldGenerateThreeFrameCurve()
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = 30.0f;

            GameObject obj2 = prefab1.transform.Find("Object2").gameObject;
            GameObject obj3 = obj2.transform.Find("Object3").gameObject;
            GameObject obj6 = obj3.transform.Find("Object6").gameObject;
            GameObject obj7 = obj6.transform.Find("Object7").gameObject;

            AnimationUtils.SetGameObjectEnabledCurves(clip, new GameObject[] { obj2, obj3, obj6, obj7 }, GenerateDummyThreeFrameCurve());

            //For debug purposes
            MakeDebugAnimAsset(clip, "SetGameObjectEnabledCurves_ShouldGenerateThreeFrameCurve");

            Assert.AreEqual(2.0f / 30.0f, clip.length);

            AssertValidIsActiveThreeFrameCurve(clip, "Object2", "m_IsActive", typeof(GameObject));
            AssertValidIsActiveThreeFrameCurve(clip, "Object2/Object3", "m_IsActive", typeof(GameObject));
            AssertValidIsActiveThreeFrameCurve(clip, "Object2/Object3/Object6", "m_IsActive", typeof(GameObject));
            AssertValidIsActiveThreeFrameCurve(clip, "Object2/Object3/Object6/Object7", "m_IsActive", typeof(GameObject));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Chocopoi.AvatarLib.Animations;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace Chocopoi.AvatarLib.Tests
{
    public class AnimationUtilsTest : EditorTestBase
    {
        private GameObject prefab1;
        private GameObject prefab2;
        private AnimatorController animator1;

        public override void SetUp()
        {
            base.SetUp();
            prefab1 = InstantiateEditorTestPrefab("ALTest_Object1.prefab");
            prefab2 = InstantiateEditorTestPrefab("ALTest_Object2.prefab");
            animator1 = LoadEditorTestAsset<AnimatorController>("ALTest_Animator.controller");
        }

        private static void MakeDebugAnimAsset(AnimationClip clip, string testName)
        {
            // #if AVATARLIB_TEST_DEBUG
            //             AssetDatabase.CreateAsset(clip, GeneratedAssetsPath + "/Test_Debug_" + testName + ".anim");
            // #endif
        }

        private static void MakeDebugAnimatorAsset(AnimatorController controller, string testName)
        {
            // #if AVATARLIB_TEST_DEBUG
            //             AssetDatabase.CreateAsset(controller, GeneratedAssetsPath + "/Test_Debug_Animator_" + testName + ".controller");
            // #endif
        }

        #region GetRelativePath
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
        #endregion

        #region SetSingleFrameGameObjectEnabledCurves
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
        #endregion

        #region SetSingleFrameComponentEnabledCurves
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
        #endregion

        #region SetGameObjectEnabledCurves
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
        #endregion

        #region RemoveAnimatorLayers
        //
        // RemoveAnimatorLayers
        //
        [Test]
        public void RemoveAnimatorLayers_ShouldRemoveOnlyRegexMatches()
        {
            AnimationUtils.RemoveAnimatorLayers(animator1, "^testTestTEST.");
            Assert.AreEqual(4, animator1.layers.Length);
        }
        #endregion

        #region RemoveAnimatorParameters
        //
        // RemoveAnimatorLayers
        //
        [Test]
        public void RemoveAnimatorParameters_ShouldRemoveOnlyRegexMatches()
        {
            AnimationUtils.RemoveAnimatorParameters(animator1, ".+para$");
            Assert.AreEqual(3, animator1.parameters.Length);
        }
        #endregion

        #region AddAnimatorParameter
        //
        // AddAnimatorParameter
        //

        private static bool IsParameterOfTypeExist(AnimatorController controller, string parameter, AnimatorControllerParameterType type)
        {
            foreach (AnimatorControllerParameter p in controller.parameters)
            {
                if (p.name == parameter)
                {
                    if (p.type == type)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        [Test]
        public void AddAnimatorParameter_ShouldHaveAnIntegerParameter()
        {
            AnimationUtils.AddAnimatorParameter(animator1, "test1para", 0);
            Assert.True(IsParameterOfTypeExist(animator1, "test1para", AnimatorControllerParameterType.Int));
        }

        [Test]
        public void AddAnimatorParameter_ShouldHaveAFloatParameter()
        {
            AnimationUtils.AddAnimatorParameter(animator1, "test2para", 0.0f);
            Assert.True(IsParameterOfTypeExist(animator1, "test2para", AnimatorControllerParameterType.Float));
        }

        [Test]
        public void AddAnimatorParameter_ShouldHaveABoolParameter()
        {
            AnimationUtils.AddAnimatorParameter(animator1, "test3para", false);
            Assert.True(IsParameterOfTypeExist(animator1, "test3para", AnimatorControllerParameterType.Bool));
        }
        #endregion

        #region CopyAnimatorStateFields
        //
        // CopyAnimatorStateFields
        //

        private static AnimatorState GenerateDummyState()
        {
            AnimatorState newState = new AnimatorState
            {
                behaviours = new StateMachineBehaviour[] {
                    null
                },
                cycleOffset = 4,
                cycleOffsetParameter = "alsogood",
                cycleOffsetParameterActive = false,
                iKOnFeet = false,
                mirror = true,
                mirrorParameter = "fakegood",
                mirrorParameterActive = true,
                speed = 2,
                speedParameter = "",
                speedParameterActive = false,
                timeParameter = "timepara",
                timeParameterActive = true
            };
            return newState;
        }

        [Test]
        public void CopyAnimatorStateFields_ShouldCopyFieldsCorrectly()
        {
            AnimatorState refState = GenerateDummyState();
            AnimatorState newState = new AnimatorState();

            AnimationUtils.CopyAnimatorStateFields(refState, newState);

            Assert.AreEqual(refState.behaviours, newState.behaviours);
            Assert.AreEqual(refState.cycleOffset, newState.cycleOffset);
            Assert.AreEqual(refState.cycleOffsetParameter, newState.cycleOffsetParameter);
            Assert.AreEqual(refState.cycleOffsetParameterActive, newState.cycleOffsetParameterActive);
            Assert.AreEqual(refState.iKOnFeet, newState.iKOnFeet);
            Assert.AreEqual(refState.mirror, newState.mirror);
            Assert.AreEqual(refState.mirrorParameter, newState.mirrorParameter);
            Assert.AreEqual(refState.mirrorParameterActive, newState.mirrorParameterActive);
            Assert.AreEqual(refState.speed, newState.speed);
            Assert.AreEqual(refState.speedParameter, newState.speedParameter);
            Assert.AreEqual(refState.speedParameterActive, newState.speedParameterActive);
            Assert.AreEqual(refState.timeParameter, newState.timeParameter);
            Assert.AreEqual(refState.timeParameterActive, newState.timeParameterActive);
        }
        #endregion

        #region CopyAnimatorTransitionFields
        //
        // CopyAnimatorTransitionFields
        //
        private static AnimatorStateTransition GenerateDummyStateTransition()
        {
            AnimatorStateTransition newTransition = new AnimatorStateTransition
            {
                canTransitionToSelf = false,
                duration = 1,
                exitTime = 0.1f,
                hasExitTime = true,
                hasFixedDuration = true,
                //newTransition.interruptionSource = null;
                isExit = false,
                mute = false,
                offset = 3,
                orderedInterruption = true,
                solo = false,
                conditions = new AnimatorCondition[] { new AnimatorCondition() }
            };
            return newTransition;
        }

        [Test]
        public void CopyAnimatorTransitionFields_ShouldCopyFieldsCorrectly()
        {
            AnimatorStateTransition refTransition = GenerateDummyStateTransition();
            AnimatorStateTransition newTransition = new AnimatorStateTransition();

            AnimationUtils.CopyAnimatorStateTransitionFields(refTransition, newTransition);

            Assert.AreEqual(refTransition.canTransitionToSelf, newTransition.canTransitionToSelf);
            Assert.AreEqual(refTransition.duration, newTransition.duration);
            Assert.AreEqual(refTransition.exitTime, newTransition.exitTime);
            Assert.AreEqual(refTransition.hasExitTime, newTransition.hasExitTime);
            Assert.AreEqual(refTransition.hasFixedDuration, newTransition.hasFixedDuration);
            Assert.AreEqual(refTransition.isExit, newTransition.isExit);
            Assert.AreEqual(refTransition.mute, newTransition.mute);
            Assert.AreEqual(refTransition.offset, newTransition.offset);
            Assert.AreEqual(refTransition.orderedInterruption, newTransition.orderedInterruption);
            Assert.AreEqual(refTransition.solo, newTransition.solo);
            Assert.AreEqual(refTransition.conditions, newTransition.conditions);
        }
        #endregion

        #region IsAnimatorParameterWithTypeExist
        //
        // IsAnimatorParameterWithTypeExist
        //

        [Test]
        public void IsAnimatorParameterWithTypeExist_ShouldReturnCorrectValues()
        {
            AnimationUtils.AddAnimatorParameter(animator1, "type1para", true);
            Assert.True(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type1para", AnimatorControllerParameterType.Bool));
            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type1para", AnimatorControllerParameterType.Int));
            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type1para", AnimatorControllerParameterType.Float));

            AnimationUtils.AddAnimatorParameter(animator1, "type2para", 1);
            Assert.True(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type2para", AnimatorControllerParameterType.Int));
            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type2para", AnimatorControllerParameterType.Bool));
            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type2para", AnimatorControllerParameterType.Float));

            AnimationUtils.AddAnimatorParameter(animator1, "type3para", 0.0f);
            Assert.True(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type3para", AnimatorControllerParameterType.Float));
            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type3para", AnimatorControllerParameterType.Bool));
            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type3para", AnimatorControllerParameterType.Int));

            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type4para", AnimatorControllerParameterType.Bool));

            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type5para", AnimatorControllerParameterType.Int));

            Assert.False(AnimationUtils.IsAnimatorParameterWithTypeExist(animator1, "type6para", AnimatorControllerParameterType.Float));
        }
        #endregion

        #region GenerateAnyStateLayer
        //
        // GenerateAnyStateLayer
        //

        [Test]
        public void GenerateAnyStateLayer_ShouldCreateWithNoErrors()
        {
            // TODO: asserts for this unit test
            AnimatorController animator = new AnimatorController();

            AnimationUtils.AddAnimatorParameter(animator, "anystate1para", 0);

            AnimationClip motion1 = new AnimationClip();
            motion1.name = "Motion1";
            AnimationClip motion2 = new AnimationClip();
            motion2.name = "Motion2";
            AnimationClip motion3 = new AnimationClip();
            motion3.name = "Motion3";

            Dictionary<int, Motion> pairs = new Dictionary<int, Motion>();

            pairs.Add(0, motion1);
            pairs.Add(1, motion2);
            pairs.Add(2, motion3);

            AnimationUtils.GenerateAnyStateLayer(animator, "AnyStateTest", "anystate1para", pairs, true);

            MakeDebugAnimatorAsset(animator, "GenerateAnyStateLayer_ShouldCreateWithNoErrors");
        }
        #endregion

        #region GenerateSingleToggleLayer
        //
        // GenerateSingleToggleLayer
        //

        [Test]
        public void GenerateSingleToggleLayer_ShouldCreateWithNoErrors()
        {
            // TODO: asserts for this unit test
            AnimatorController animator = new AnimatorController();

            AnimationUtils.AddAnimatorParameter(animator, "toggle1para", false);

            AnimationClip motion1 = new AnimationClip();
            motion1.name = "Motion1";
            AnimationClip motion2 = new AnimationClip();
            motion2.name = "Motion2";

            AnimationUtils.GenerateSingleToggleLayer(animator, "SingleToggleTest", "toggle1para", motion1, motion2, true);

            MakeDebugAnimatorAsset(animator, "GenerateSingleToggleLayer_ShouldCreateWithNoErrors");
        }
        #endregion

        #region GenerateSingleMotionTimeLayer
        //
        // GenerateSingleMotionTimeLayer
        //

        [Test]
        public void GenerateSingleMotionTimeLayer_ShouldCreateWithNoErrors()
        {
            // TODO: asserts for this unit test
            AnimatorController animator = new AnimatorController();

            AnimationUtils.AddAnimatorParameter(animator, "time1para", 0.0f);

            AnimationClip motion1 = new AnimationClip();
            motion1.name = "Motion1";

            AnimationUtils.GenerateSingleMotionTimeLayer(animator, "MotionTimeTest", "time1para", motion1, true);

            MakeDebugAnimatorAsset(animator, "GenerateSingleMotionTimeLayer_ShouldCreateWithNoErrors");
        }
        #endregion
    }
}

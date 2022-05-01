using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Animations;
using UnityEngine;

namespace Chocopoi.AvatarLib.Animations
{
    /// <summary>
    /// An utility class for creating and modifying animations and animators
    /// </summary>
    class AnimationUtils
    {
        //Referenced and modified from https://answers.unity.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
        /// <summary>
        /// Gets the relative path to the provided Transform from the very first parent (e.g. Avatar/Armature/Hips/Spine/Chest)
        /// </summary>
        /// <param name="transform">The Transform for finding the relative path</param>
        /// <param name="untilTransform">Finds the relative path until this transform (exclusive). Keeping it <code>null</code> will find the very first parent</param>
        /// <param name="prefix">A prefix to be put before the path</param>
        /// <param name="suffix">A suffix to be put after the path</param>
        /// <returns></returns>
        public static string GetRelativePath(Transform transform, Transform untilTransform = null, string prefix = "", string suffix = "")
        {
            string path = transform.name;
            while (true)
            {
                transform = transform.parent;

                if (transform.parent == null || (untilTransform != null && transform == untilTransform))
                {
                    break;
                }

                path = transform.name + "/" + path;
            }
            return prefix + path + suffix;
        }

        /// <summary>
        /// Sets GameObject active (m_IsActive) property enabled or not enabled in the first frame only (time = 0.0f)
        /// </summary>
        /// <param name="clip">The AnimationClip to modify</param>
        /// <param name="gameObjects">GameObjects to animate</param>
        /// <param name="enabled">Set to enabled or not</param>
        /// <param name="prefix">Prefix to be added to the relative path</param>
        /// <param name="suffix">Suffix to be added to the relative path</param>
        public static void SetSingleFrameGameObjectEnabledCurves(AnimationClip clip, GameObject[] gameObjects, bool enabled, string prefix = "", string suffix = "")
        {
            foreach (GameObject obj in gameObjects)
            {
                clip.SetCurve(GetRelativePath(obj.transform, null, prefix, suffix), typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0.0f, 0.0f, enabled ? 1.0f : 0.0f));
            }
        }

        /// <summary>
        /// Sets GameObject active (m_IsActive) property to the provided curve
        /// </summary>
        /// <param name="clip">The AnimationClip to modify</param>
        /// <param name="gameObjects">GameObjects to animate</param>
        /// <param name="curve">The curve to set</param>
        /// <param name="prefix">Prefix to be added to the relative path</param>
        /// <param name="suffix">Suffix to be added to the relative path</param>
        public static void SetGameObjectEnabledCurves(AnimationClip clip, GameObject[] gameObjects, AnimationCurve curve, string prefix = "", string suffix = "")
        {
            foreach (GameObject obj in gameObjects)
            {
                clip.SetCurve(GetRelativePath(obj.transform, null, prefix, suffix), typeof(GameObject), "m_IsActive", curve);
            }
        }

        /// <summary>
        /// Sets component enabled (m_Enabled) property enabled or not enabled in the first frame only (time = 0.0f)
        /// </summary>
        /// <param name="clip">The AnimationClip to modify</param>
        /// <param name="comps">Components to animate</param>
        /// <param name="enabled">Set to enabled or not</param>
        /// <param name="prefix">Prefix to be added to the relative path</param>
        /// <param name="suffix">Suffix to be added to the relative path</param>
        public static void SetSingleFrameComponentEnabledCurves(AnimationClip clip, Component[] comps, bool enabled, string prefix = "", string suffix = "")
        {
            foreach (Component comp in comps)
            {
                clip.SetCurve(GetRelativePath(comp.transform, null, prefix, suffix), typeof(GameObject), "m_Enabled", AnimationCurve.Constant(0.0f, 0.0f, enabled ? 1.0f : 0.0f));
            }
        }

        /// <summary>
        /// Sets component enabled (m_Enabled) property to the provided curve
        /// </summary>
        /// <param name="clip">The AnimationClip to modify</param>
        /// <param name="comps">Components to animate</param>
        /// <param name="curve">The curve to set</param>
        /// <param name="prefix">Prefix to be added to the relative path</param>
        /// <param name="suffix">Suffix to be added to the relative path</param>
        public static void SetComponentEnabledCurves(AnimationClip clip, Component[] comps, AnimationCurve curve, string prefix = "", string suffix = "")
        {
            foreach (Component comp in comps)
            {
                clip.SetCurve(GetRelativePath(comp.transform, null, prefix, suffix), comp.GetType(), "m_Enabled", curve);
            }
        }

        /// <summary>
        /// Removes animator layers with name matching with regex
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="regex">The regex pattern</param>
        public static void RemoveAnimatorLayers(AnimatorController controller, string regex)
        {
            RemoveAnimatorLayers(controller, new Regex(regex));
        }

        /// <summary>
        /// Removes animator layers with name matching with regex
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="regex">The regex pattern</param>
        public static void RemoveAnimatorLayers(AnimatorController controller, Regex regex)
        {
            //Remove all existing layers matching regex
            for (int i = 0; i < controller.layers.Length; i++)
            {
                if (regex.IsMatch(controller.layers[i].name))
                {
                    controller.RemoveLayer(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// A handy way to add an animator parameter with boolean type
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="name">the parameter name</param>
        /// <param name="defaultValue">The default value to be set</param>
        public static void AddAnimatorParameter(AnimatorController controller, string name, bool defaultValue)
        {
            controller.AddParameter(new AnimatorControllerParameter()
            {
                name = name,
                type = AnimatorControllerParameterType.Bool,
                defaultBool = defaultValue
            });
        }

        /// <summary>
        /// A handy way to add an animator parameter with int type
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="name">the parameter name</param>
        /// <param name="defaultValue">The default value to be set</param>
        public static void AddAnimatorParameter(AnimatorController controller, string name, int defaultValue)
        {
            controller.AddParameter(new AnimatorControllerParameter()
            {
                name = name,
                type = AnimatorControllerParameterType.Int,
                defaultInt = defaultValue
            });
        }

        /// <summary>
        /// A handy way to add an animator parameter with float type
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="name">the parameter name</param>
        /// <param name="defaultValue">The default value to be set</param>
        public static void AddAnimatorParameter(AnimatorController controller, string name, float defaultValue)
        {
            controller.AddParameter(new AnimatorControllerParameter()
            {
                name = name,
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue
            });
        }

        /// <summary>
        /// Removes animator parameters with name matching with regex
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="regex">The regex pattern</param>
        public static void RemoveAnimatorParameters(AnimatorController controller, string regex)
        {
            RemoveAnimatorParameters(controller, new Regex(regex));
        }

        /// <summary>
        /// Removes animator parameters with name matching with regex
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="regex">The regex pattern</param>
        public static void RemoveAnimatorParameters(AnimatorController controller, Regex regex)
        {
            //Remove all existing parameters matching regex

            for (int i = 0; i < controller.parameters.Length; i++)
            {
                if (regex.IsMatch(controller.parameters[i].name))
                {
                    controller.RemoveParameter(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Copies all fields (except for AnimationClip and write default values) from the <code>oldState</code> to <code>newState</code>
        /// </summary>
        /// <param name="oldState">Old state</param>
        /// <param name="newState">New state</param>
        public static void CopyAnimatorStateFields(AnimatorState oldState, AnimatorState newState)
        {
            newState.behaviours = oldState.behaviours;
            newState.cycleOffset = oldState.cycleOffset;
            newState.cycleOffsetParameter = oldState.cycleOffsetParameter;
            newState.cycleOffsetParameterActive = oldState.cycleOffsetParameterActive;
            newState.iKOnFeet = oldState.iKOnFeet;
            newState.mirror = oldState.mirror;
            newState.mirrorParameter = oldState.mirrorParameter;
            newState.mirrorParameterActive = oldState.mirrorParameterActive;
            newState.speed = oldState.speed;
            newState.speedParameter = oldState.speedParameter;
            newState.speedParameterActive = oldState.speedParameterActive;
            newState.timeParameter = oldState.timeParameter;
            newState.timeParameterActive = oldState.timeParameterActive;
        }

        /// <summary>
        /// Copies all fields from the <code>oldTransition</code> to <code>newTransition</code>
        /// </summary>
        /// <param name="oldTransition">Old transition</param>
        /// <param name="newTransition">New transition</param>
        public static void CopyAnimatorTransitionFields(AnimatorStateTransition oldTransition, AnimatorStateTransition newTransition)
        {
            newTransition.canTransitionToSelf = oldTransition.canTransitionToSelf;
            newTransition.duration = oldTransition.duration;
            newTransition.exitTime = oldTransition.exitTime;
            newTransition.hasExitTime = oldTransition.hasExitTime;
            newTransition.hasFixedDuration = oldTransition.hasFixedDuration;
            newTransition.interruptionSource = oldTransition.interruptionSource;
            newTransition.isExit = oldTransition.isExit;
            newTransition.mute = oldTransition.mute;
            newTransition.offset = oldTransition.offset;
            newTransition.orderedInterruption = oldTransition.orderedInterruption;
            newTransition.solo = oldTransition.solo;
            newTransition.conditions = oldTransition.conditions;
        }

        /// <summary>
        /// Sets all fields of <code>newState</code> to default configuration
        /// </summary>
        /// <param name="newState">New state</param>
        public static void SetAnimatorStateFieldsToDefault(AnimatorState newState)
        {
            newState.behaviours = new StateMachineBehaviour[] { };
            newState.cycleOffset = 0;
            newState.cycleOffsetParameter = "";
            newState.cycleOffsetParameterActive = false;
            newState.iKOnFeet = false;
            newState.mirror = false;
            newState.mirrorParameter = "";
            newState.mirrorParameterActive = false;
            newState.speed = 1;
            newState.speedParameter = "";
            newState.speedParameterActive = false;
            newState.timeParameter = "";
            newState.timeParameterActive = false;
        }

        /// <summary>
        /// Sets all fields of <code>newTransition</code> to default configuration
        /// </summary>
        /// <param name="newTransition">newTransition</param>
        public static void SetAnimatorTransitionFieldsToDefault(AnimatorStateTransition newTransition)
        {
            newTransition.canTransitionToSelf = true;
            newTransition.duration = 0;
            newTransition.exitTime = 0;
            newTransition.hasExitTime = true;
            newTransition.hasFixedDuration = true;
            //newTransition.interruptionSource = null;
            newTransition.isExit = false;
            newTransition.mute = false;
            newTransition.offset = 0;
            newTransition.orderedInterruption = true;
            newTransition.solo = false;
            newTransition.conditions = new AnimatorCondition[] { };
        }

        public static bool IsAnimatorParameterWithTypeExist(AnimatorController controller, string parameter, System.Type type)
        {
            foreach (AnimatorControllerParameter p in controller.parameters)
            {
                if (p.name == parameter && p.GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates an animator layer in the <code>AnyState--{condition}-->State</code> structure
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="layerName">The new layer name</param>
        /// <param name="parameter">The parameter that controls the AnyState conditions</param>
        /// <param name="pairs">Pairs of parameter values and Motion, the matching parameter value will make the layer change to the respective Motion. The value 0 will be the default state.</param>
        /// <param name="writeDefaults">Write Default Values on every states in this animator layer</param>
        /// <param name="referenceState">Reference state values, every fields will be copied from here except for the AnimationClip. Keep it <code>null</code> to use default configuration.</param>
        /// <param name="referenceTransition">Reference transition values, every fields will be copied, including conditions. Keep it <code>null</code> to use default configuration.</param>
        public static void GenerateAnyStateLayer(AnimatorController controller, string layerName, string parameter, Dictionary<int, Motion> pairs, bool writeDefaults, AnimatorState referenceState = null, AnimatorStateTransition referenceTransition = null)
        {
            if (!IsAnimatorParameterWithTypeExist(controller, parameter, typeof(int)))
            {
                throw new ParameterNotExistException(parameter, typeof(int));
            }

            AnimatorControllerLayer newLayer = new AnimatorControllerLayer
            {
                name = layerName,
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };

            if (!pairs.Keys.Contains(0))
            {
                throw new System.Exception("No default state value (0) is found in the dictionary provided. An animation for value 0 is required.");
            }

            foreach (int val in pairs.Keys)
            {
                // Create state

                pairs.TryGetValue(val, out Motion motion);
                AnimatorState newState = newLayer.stateMachine.AddState(motion.name);

                if (val == 0)
                {
                    newLayer.stateMachine.defaultState = newState;
                }

                if (referenceState != null)
                {
                    CopyAnimatorStateFields(referenceState, newState);
                }
                else
                {
                    SetAnimatorStateFieldsToDefault(newState);
                }

                newState.writeDefaultValues = writeDefaults;
                newState.motion = motion;

                // create AnyState transition

                AnimatorStateTransition newTransition = newLayer.stateMachine.AddAnyStateTransition(newState);

                if (referenceTransition != null)
                {
                    CopyAnimatorTransitionFields(referenceTransition, newTransition);
                }
                else
                {
                    SetAnimatorTransitionFieldsToDefault(newTransition);
                }

                newTransition.AddCondition(AnimatorConditionMode.Equals, val, parameter);
            }

            controller.AddLayer(newLayer);
        }

        /// <summary>
        /// Generates a single toggle layer, controlled by a single boolean parameter
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="layerName">The new layer name</param>
        /// <param name="parameter">The boolean parameter that controls the AnyState conditions</param>
        /// <param name="offMotion">The motion to be used if the parameter is true (false if inverted). You can set this to <code>null</code> if Write Defaults is on.</param>
        /// <param name="onMotion">The motion to be used if the parameter is false (true if inverted)</param>
        /// <param name="writeDefaults">Write Default Values on every states in this animator layer</param>
        /// <param name="inverted">Invert the logic, false -> onMotion, true -> offMotion</param>
        /// <param name="referenceState">Reference state values, every fields will be copied from here except for the AnimationClip. Keep it <code>null</code> to use default configuration.</param>
        /// <param name="referenceTransition">Reference transition values, every fields will be copied, including conditions. Keep it <code>null</code> to use default configuration.</param>
        public static void GenerateSingleToggleLayer(AnimatorController controller, string layerName, string parameter, Motion offMotion, Motion onMotion, bool writeDefaults, bool inverted = false, AnimatorState referenceState = null, AnimatorStateTransition referenceTransition = null)
        {
            if (!IsAnimatorParameterWithTypeExist(controller, parameter, typeof(bool)))
            {
                throw new ParameterNotExistException(parameter, typeof(bool));
            }

            AnimatorControllerLayer newLayer = new AnimatorControllerLayer
            {
                name = layerName,
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };

            // create states

            AnimatorState offState = newLayer.stateMachine.AddState(offMotion.name);
            AnimatorState onState = newLayer.stateMachine.AddState(onMotion.name);
            newLayer.stateMachine.defaultState = offState;

            // copy state fields

            if (referenceState != null)
            {
                CopyAnimatorStateFields(referenceState, offState);
                CopyAnimatorStateFields(referenceState, onState);
            }
            else
            {
                SetAnimatorStateFieldsToDefault(offState);
                SetAnimatorStateFieldsToDefault(onState);
            }

            offState.writeDefaultValues = writeDefaults;
            onState.writeDefaultValues = writeDefaults;

            offState.motion = offMotion;
            onState.motion = onMotion;

            AnimatorStateTransition offStateTransition = newLayer.stateMachine.AddAnyStateTransition(offState);
            AnimatorStateTransition onStateTransition = newLayer.stateMachine.AddAnyStateTransition(onState);

            // copy transition fields

            if (referenceTransition != null)
            {
                CopyAnimatorTransitionFields(referenceTransition, offStateTransition);
                CopyAnimatorTransitionFields(referenceTransition, onStateTransition);
            }
            else
            {
                SetAnimatorTransitionFieldsToDefault(offStateTransition);
                SetAnimatorTransitionFieldsToDefault(onStateTransition);
            }

            // add condition

            if (inverted)
            {
                offStateTransition.AddCondition(AnimatorConditionMode.If, 0, parameter);
                onStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0, parameter);
            }
            else
            {
                offStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0, parameter);
                onStateTransition.AddCondition(AnimatorConditionMode.If, 0, parameter);
            }
        }

        /// <summary>
        /// Generates single motion time layer, that the layer only contains a single Motion controlled by a motion time parameter.
        /// </summary>
        /// <param name="controller">The AnimatorController to be modified</param>
        /// <param name="layerName">The new layer name</param>
        /// <param name="motionTimeParameter">Motion time parameter</param>
        /// <param name="motion">The single motion of the layer</param>
        /// <param name="writeDefaults">Write Default Values in this animator layer</param>
        /// <param name="referenceState">Reference state values, every fields will be copied from here except for the AnimationClip. Keep it <code>null</code> to use default configuration.</param>
        public static void GenerateSingleMotionTimeLayer(AnimatorController controller, string layerName, string motionTimeParameter, Motion motion, bool writeDefaults, AnimatorState referenceState = null)
        {
            if (!IsAnimatorParameterWithTypeExist(controller, motionTimeParameter, typeof(float)))
            {
                throw new ParameterNotExistException(motionTimeParameter, typeof(float));
            }

            AnimatorControllerLayer newLayer = new AnimatorControllerLayer
            {
                name = layerName,
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine()
            };

            AnimatorState state = newLayer.stateMachine.AddState(motion.name);
            newLayer.stateMachine.defaultState = state;

            if (referenceState != null)
            {
                CopyAnimatorStateFields(referenceState, state);
            }
            else
            {
                SetAnimatorStateFieldsToDefault(state);
            }

            state.motion = motion;
            state.timeParameter = motionTimeParameter;
            state.timeParameterActive = true;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif

namespace Chocopoi.AvatarLib.Animations
{
    public class AnimatorMerger
    {
        public enum WriteDefaultsMode
        {
            DoNothing = 0,
            On = 1,
            Off = 2
        }

        private AnimatorController _mergedAnimator;
        private Dictionary<System.Tuple<string, AnimatorStateMachine>, AnimatorStateMachine> _stateMachineCache;
        private Dictionary<string, AnimatorControllerParameter> _parameters;
        private Dictionary<string, AnimatorControllerLayer> _layers;
        private Dictionary<string, int> _layerNameDuplicateCount;
        private bool _saveToAssets;

        public AnimatorMerger(string mergedAnimatorAssetPath = null)
        {
            _mergedAnimator = new AnimatorController();

            if (mergedAnimatorAssetPath != null)
            {
                _saveToAssets = true;
                AssetDatabase.CreateAsset(_mergedAnimator, mergedAnimatorAssetPath);
                AssetDatabase.SaveAssets();
            }

            _stateMachineCache = new Dictionary<System.Tuple<string, AnimatorStateMachine>, AnimatorStateMachine>();
            _parameters = new Dictionary<string, AnimatorControllerParameter>();
            _layers = new Dictionary<string, AnimatorControllerLayer>();
            _layerNameDuplicateCount = new Dictionary<string, int>();
        }

        public void AddAnimator(string rebasePath, AnimatorController animator, WriteDefaultsMode writeDefaultsMode = WriteDefaultsMode.DoNothing)
        {
            // deep copy parameters
            DeepCopyParameters(animator);

            // deep copy layers
            var layers = animator.layers;
            for (var i = 0; i < layers.Length; i++)
            {
                var originalLayer = layers[i];
                var weight = i == 0 ? 1 : originalLayer.defaultWeight;

                var copyCache = new Dictionary<Object, Object>();

                var newLayerName = originalLayer.name;
                if (_layers.ContainsKey(originalLayer.name))
                {
                    if (!_layerNameDuplicateCount.TryGetValue(originalLayer.name, out var count))
                    {
                        count = 1;
                    }
                    newLayerName += "_" + count;
                    _layerNameDuplicateCount[originalLayer.name] = ++count;
                }

                // create copy
                var newLayer = new AnimatorControllerLayer()
                {
                    name = newLayerName,
                    defaultWeight = weight,
                    blendingMode = originalLayer.blendingMode,
                    iKPass = originalLayer.iKPass,
                    avatarMask = originalLayer.avatarMask,
                    syncedLayerIndex = originalLayer.syncedLayerIndex,
                    syncedLayerAffectsTiming = originalLayer.syncedLayerAffectsTiming,
                    stateMachine = DeepCopyStateMachineWithRebasing(rebasePath, originalLayer.stateMachine, copyCache),
                };

                HandleWriteDefaults(newLayer.stateMachine, writeDefaultsMode);

                if (originalLayer.syncedLayerIndex != -1 && originalLayer.syncedLayerIndex >= 0 && originalLayer.syncedLayerIndex < layers.Length)
                {
                    HandleSyncedLayer(originalLayer, layers[originalLayer.syncedLayerIndex], newLayer, copyCache);
                }

                _layers.Add(newLayer.name, newLayer);
            }
        }

        public AnimatorController Merge()
        {
            _mergedAnimator.layers = _layers.Values.ToArray();
            _mergedAnimator.parameters = _parameters.Values.ToArray();
            if (_saveToAssets)
            {
                EditorUtility.SetDirty(_mergedAnimator);
                AssetDatabase.SaveAssets();
            }
            return _mergedAnimator;
        }

        private void HandleSyncedLayer(AnimatorControllerLayer originalLayer, AnimatorControllerLayer baseLayer, AnimatorControllerLayer newLayer, Dictionary<Object, Object> copyCache)
        {
            var it = WalkStateMachine(baseLayer.stateMachine);
            foreach (var state in it)
            {
                var animState = (AnimatorState)copyCache[state];

                // copy override motion
                var motion = originalLayer.GetOverrideMotion(state);
                if (motion != null)
                {
                    newLayer.SetOverrideMotion(animState, motion);
                }

                // copy override behaviours
                var behaviours = originalLayer.GetOverrideBehaviours(state);
                if (behaviours != null)
                {
                    var copy = new StateMachineBehaviour[behaviours.Length];
                    behaviours.CopyTo(copy, 0);

                    for (var i = 0; i < copy.Length; i++)
                    {
                        behaviours[i] = GenericDeepCopy(behaviours[i]);
                        HandleAnimatorBehaviour(behaviours[i]);
                    }

                    newLayer.SetOverrideBehaviours(animState, copy);
                }
            }
            newLayer.syncedLayerIndex += _layers.Count;
        }

#if VRC_SDK_VRCSDK3
        private static bool IsVRCProxyAnim(Motion m)
        {
            if (m == null) return false;
            var animPath = AssetDatabase.GetAssetPath(m);
            return animPath != null && animPath != "" && animPath.Contains("/Animation/ProxyAnim/proxy");
        }
#endif

        private static string RebaseCurvePath(EditorCurveBinding curveBinding, string rebasePath)
        {
            if (rebasePath == "" && curveBinding.type == typeof(Animator))
            {
                return "";
            }

            return curveBinding.path == "" ?
                rebasePath :
                (rebasePath + "/" + curveBinding.path);
        }

        private void HandleWriteDefaults(AnimatorStateMachine stateMachine, WriteDefaultsMode writeDefaultsMode)
        {
            if (writeDefaultsMode == WriteDefaultsMode.DoNothing)
            {
                return;
            }

            // iterate all states
            foreach (var state in stateMachine.states)
            {
                state.state.writeDefaultValues = writeDefaultsMode == WriteDefaultsMode.On;
            }

            // iterate all child state machines
            foreach (var childStateMachine in stateMachine.stateMachines)
            {
                HandleWriteDefaults(childStateMachine.stateMachine, writeDefaultsMode);
            }
        }

        private AnimatorStateMachine DeepCopyStateMachineWithRebasing(string rebasePath, AnimatorStateMachine stateMachine, Dictionary<Object, Object> copyCache = null)
        {
            if (copyCache == null)
            {
                copyCache = new Dictionary<Object, Object>();
            }

            // attempt to find cache to not copy again
            var key = new System.Tuple<string, AnimatorStateMachine>(rebasePath, stateMachine);
            if (_stateMachineCache.TryGetValue(key, out var cachedStateMachine))
            {
                return cachedStateMachine;
            }

            var newStateMachine = GenericDeepCopy(stateMachine, (Object obj) =>
            {
                if (!(obj is AnimationClip))
                {
                    return null;
                }

                var anim = (AnimationClip)obj;
                if (rebasePath == "") return anim;
#if VRC_SDK_VRCSDK3
                if (IsVRCProxyAnim(anim)) return anim;
#endif

                // create new copy
                var newAnim = new AnimationClip
                {
                    name = anim.name + "_Rebase",
                    legacy = anim.legacy,
                    frameRate = anim.frameRate,
                    localBounds = anim.localBounds,
                    wrapMode = anim.wrapMode
                };
                AnimationUtility.SetAnimationClipSettings(newAnim, AnimationUtility.GetAnimationClipSettings(anim));

                if (_saveToAssets)
                {
                    AssetDatabase.AddObjectToAsset(newAnim, _mergedAnimator);
                }

                // object reference curves
                var objRefBindings = AnimationUtility.GetObjectReferenceCurveBindings(anim);
                foreach (var objRefBinding in objRefBindings)
                {
                    var newObjRefBinding = objRefBinding;
                    newObjRefBinding.path = RebaseCurvePath(objRefBinding, rebasePath);
                    AnimationUtility.SetObjectReferenceCurve(newAnim, newObjRefBinding, AnimationUtility.GetObjectReferenceCurve(anim, objRefBinding));
                }

                // curves
                var curveBindings = AnimationUtility.GetCurveBindings(anim);
                foreach (var curveBinding in curveBindings)
                {
                    newAnim.SetCurve(RebaseCurvePath(curveBinding, rebasePath), curveBinding.type, curveBinding.propertyName, AnimationUtility.GetEditorCurve(anim, curveBinding));
                }

                return newAnim;
            }, copyCache);

            // update animator behaviours
            var it = WalkStateMachine(stateMachine);
            foreach (var state in it)
            {
                foreach (var behaviour in state.behaviours)
                {
                    HandleAnimatorBehaviour(behaviour);
                }
            }

            // save to cache
            _stateMachineCache[key] = newStateMachine;
            return newStateMachine;
        }

        private void HandleAnimatorBehaviour(StateMachineBehaviour behaviour)
        {
#if VRC_SDK_VRCSDK3
            if (behaviour is VRCAnimatorLayerControl control)
            {
                // update control layer index
                control.layer = _layers.Count;
            }
#endif
        }

        private void DeepCopyParameters(AnimatorController animator)
        {
            foreach (var param in animator.parameters)
            {
                if (_parameters.TryGetValue(param.name, out var existingType) && param.type != existingType.type)
                {
                    // throw exception if type mismatch
                    throw new ParameterMismatchException(param.name, existingType.name, param.name);
                }
                else
                {
                    // deep copy parameter
                    var newParam = new AnimatorControllerParameter
                    {
                        name = param.name,
                        type = param.type,
                        defaultBool = param.defaultBool,
                        defaultFloat = param.defaultFloat,
                        defaultInt = param.defaultInt
                    };
                    _parameters.Add(param.name, newParam);
                }
            }
        }

        private static IEnumerable<AnimatorState> WalkStateMachine(AnimatorStateMachine stateMachine, HashSet<AnimatorStateMachine> visitedStateMachines = null)
        {
            if (visitedStateMachines == null)
            {
                visitedStateMachines = new HashSet<AnimatorStateMachine>();
            }

            // do not walk visited state machines
            if (visitedStateMachines.Contains(stateMachine))
            {
                yield break;
            }
            visitedStateMachines.Add(stateMachine);

            // yield all states
            foreach (var state in stateMachine.states)
            {
                if (state.state == null)
                {
                    continue;
                }

                yield return state.state;
            }

            // recursive visit other state machines
            foreach (var childStateMachine in stateMachine.stateMachines)
            {
                if (stateMachine.stateMachines == null)
                {
                    continue;
                }

                if (visitedStateMachines.Contains(childStateMachine.stateMachine))
                {
                    continue;
                }
                visitedStateMachines.Add(childStateMachine.stateMachine);

                // walk the child state machine
                var states = WalkStateMachine(childStateMachine.stateMachine, visitedStateMachines);
                foreach (var state in states)
                {
                    yield return state;
                }
            }
        }

        private T GenericDeepCopy<T>(T originalObject, System.Func<Object, Object> genericCopyFunc = null, Dictionary<Object, Object> copyCache = null) where T : Object
        {
            if (copyCache == null)
            {
                copyCache = new Dictionary<Object, Object>();
            }

            if (originalObject == null)
            {
                return null;
            }

            var originalObjectType = originalObject.GetType();

            // do not copy these types and return original
            if (originalObject is MonoScript ||
                originalObject is ScriptableObject ||
                originalObject is Texture ||
                originalObject is Material)
            {
                return originalObject;
            }

            // only copy known types
            if (!(originalObject is Motion ||
                originalObject is AnimatorController ||
                originalObject is AnimatorStateMachine ||
                originalObject is StateMachineBehaviour ||
                originalObject is AnimatorState ||
                originalObject is AnimatorTransitionBase))
            {
                throw new System.Exception(string.Format("Unknown type detected while animator merging: {0}", originalObjectType.FullName));
            }

            // try obtain from cache
            if (copyCache.TryGetValue(originalObject, out var obj))
            {
                return (T)obj;
            }

            Object newObj;

            // attempt to copy with generic copy function
            if (genericCopyFunc != null)
            {
                newObj = genericCopyFunc(originalObject);
                if (newObj != null)
                {
                    return (T)newObj;
                }
            }

            // initialize a new object in a generic way
            var constructor = originalObjectType.GetConstructor(System.Type.EmptyTypes);
            if (constructor != null && !(originalObject is ScriptableObject))
            {
                newObj = (T)System.Activator.CreateInstance(originalObjectType);
                // copy serialized properties
                EditorUtility.CopySerialized(originalObject, newObj);
            }
            else
            {
                newObj = Object.Instantiate(originalObject);
            }
            copyCache[originalObject] = newObj;

            // save to assets
            if (_saveToAssets)
            {
                AssetDatabase.AddObjectToAsset(newObj, _mergedAnimator);
            }

            // deep copy serialized properties
            var serializedObj = new SerializedObject(newObj);
            var it = serializedObj.GetIterator();

            bool traverseDown = true;
            while (it.Next(traverseDown))
            {
                // reset
                traverseDown = true;

                if (it.propertyType == SerializedPropertyType.String)
                {
                    // disable traversal
                    traverseDown = false;
                }
                else if (it.propertyType == SerializedPropertyType.ObjectReference)
                {
                    // recursively perform deep copy
                    it.objectReferenceValue = GenericDeepCopy(it.objectReferenceValue, genericCopyFunc, copyCache);
                }
            }

            // apply changes
            serializedObj.ApplyModifiedPropertiesWithoutUndo();

            return (T)newObj;
        }
    }
}

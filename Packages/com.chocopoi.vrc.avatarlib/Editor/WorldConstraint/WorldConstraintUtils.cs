using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Chocopoi.AvatarLib.Animations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.AvatarLib.WorldConstraint
{
    /// <summary>
    /// An utility class to create a WorldConstraint in an avatar
    /// </summary>
    public class WorldConstraintUtils
    {
        /// <summary>
        /// Creates a world constraint
        /// </summary>
        /// <param name="item">The item to be applied a world constraint on</param>
        /// <param name="root">Parent the world constraint to a Transform</param>
        /// <param name="resetTarget">The reset target to reset the world constraint to</param>
        /// <returns></returns>
        public static WorldConstraint CreateWorldConstraint(GameObject item = null, Transform root = null, Transform resetTarget = null)
        {
            // World space

            GameObject worldSpaceObj = new GameObject("WorldConstraint");

            if (root != null)
            {
                worldSpaceObj.transform.SetParent(root);
            }

            GameObject worldSpaceTargetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.chocopoi.vrc.avatarlib/Prefabs/WorldSpaceTarget.prefab");

            ParentConstraint worldSpaceParentConstraint = worldSpaceObj.AddComponent<ParentConstraint>();
            worldSpaceParentConstraint.AddSource(new ConstraintSource()
            {
                sourceTransform = worldSpaceTargetPrefab.transform,
                weight = 1
            });
            worldSpaceParentConstraint.constraintActive = true;

            // Container

            GameObject container = new GameObject("Container");
            container.transform.SetParent(worldSpaceObj.transform);

            // Reset target 

            if (resetTarget == null)
            {
                GameObject resetTargetObj = new GameObject("ResetTarget");

                if (root != null)
                {
                    resetTargetObj.transform.SetParent(root);
                }

                resetTarget = resetTargetObj.transform;
            }

            ParentConstraint resetTargetParentConstraint = container.AddComponent<ParentConstraint>();
            resetTargetParentConstraint.AddSource(new ConstraintSource()
            {
                sourceTransform = resetTarget,
                weight = 1
            });
            resetTargetParentConstraint.constraintActive = true;

            return new WorldConstraint()
            {
                WorldSpaceConstraint = worldSpaceParentConstraint,
                ResetTargetConstraint = resetTargetParentConstraint,
                Root = worldSpaceObj,
                Container = container
            };
        }

        /// <summary>
        /// Generates an animation to lock the world constraint to world space
        /// </summary>
        /// <param name="worldConstraint">The world constraint object</param>
        /// <returns>An AnimationClip</returns>
        public static AnimationClip GenerateLockAnimation(WorldConstraint worldConstraint)
        {
            AnimationClip clip = new AnimationClip();
            AnimationUtils.SetSingleFrameComponentEnabledCurves(clip, new Component[] { worldConstraint.ResetTargetConstraint }, false);
            return clip;
        }

        /// <summary>
        /// Generates an animation to unlock the world constraint from world space
        /// </summary>
        /// <param name="worldConstraint">The world constraint object</param>
        /// <returns>An AnimationClip</returns>
        public static AnimationClip GenerateUnlockAnimation(WorldConstraint worldConstraint)
        {
            AnimationClip clip = new AnimationClip();
            AnimationUtils.SetSingleFrameComponentEnabledCurves(clip, new Component[] { worldConstraint.ResetTargetConstraint }, true);
            return clip;
        }

        /// <summary>
        /// Generates an animation to set the world constraint root to be active (shown)
        /// </summary>
        /// <param name="worldConstraint">The world constraint object</param>
        /// <returns>An AnimationClip</returns>
        public static AnimationClip GenerateShowAnimation(WorldConstraint worldConstraint)
        {
            AnimationClip clip = new AnimationClip();
            AnimationUtils.SetSingleFrameGameObjectEnabledCurves(clip, new GameObject[] { worldConstraint.Root }, true);
            return clip;
        }

        /// <summary>
        /// Generates an animation to set the world constraint root to be inactive (hidden)
        /// </summary>
        /// <param name="worldConstraint">The world constraint object</param>
        /// <returns>An AnimationClip</returns>
        public static AnimationClip GenerateHideAnimation(WorldConstraint worldConstraint)
        {
            AnimationClip clip = new AnimationClip();
            AnimationUtils.SetSingleFrameGameObjectEnabledCurves(clip, new GameObject[] { worldConstraint.Root }, false);
            return clip;
        }
    }
}

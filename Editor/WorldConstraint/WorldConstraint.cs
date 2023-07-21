using UnityEngine;
using UnityEngine.Animations;

namespace Chocopoi.AvatarLib.WorldConstraint
{
    /// <summary>
    /// A class referencing to the important objects of a world constraint
    /// </summary>
    public class WorldConstraint
    {
        /// <summary>
        /// The ParentConstraint linking to the world space transform.
        /// </summary>
        public ParentConstraint WorldSpaceConstraint { get; set; }

        /// <summary>
        /// The ParentConstraint linking to the avatar transform. Set it to be inactive to leave the container in the world space.
        /// </summary>
        public ParentConstraint ResetTargetConstraint { get; set; }

        /// <summary>
        /// The world constraint root
        /// </summary>
        public GameObject Root { get; set; }

        /// <summary>
        /// The world constraint container to place objects into it
        /// </summary>
        public GameObject Container { get; set; }
    }
}

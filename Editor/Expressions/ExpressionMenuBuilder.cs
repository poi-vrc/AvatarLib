using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

#if VRC_SDK_VRCSDK3
namespace Chocopoi.AvatarLib.Expressions
{
    /// <summary>
    /// A handy builder to create/modify a VRCExpressionMenu
    /// </summary>
    public class ExpressionMenuBuilder
    {
        private VRCExpressionsMenu menu;

        private ExpressionMenuBuilder parentBuilder;

        private string parentName;

        private string parentParameterOnOpen;

        private Texture2D parentIcon;

        private float parentValueOnOpen;

        // Used for BeginNewSubMenu()
        private ExpressionMenuBuilder(ExpressionMenuBuilder parentBuilder, string parentName, string parentParameterOnOpen, Texture2D parentIcon, float parentValueOnOpen)
        {
            menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            this.parentName = parentName;
            this.parentBuilder = parentBuilder;
            this.parentParameterOnOpen = parentParameterOnOpen;
            this.parentIcon = parentIcon;
            this.parentValueOnOpen = parentValueOnOpen;
        }

        /// <summary>
        /// Initializes with a new VRCExpressionMenu
        /// </summary>
        public ExpressionMenuBuilder()
        {
            menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
        }

        /// <summary>
        /// Initializes with an existing VRCExpressionMenu
        /// </summary>
        /// <param name="menu"></param>
        public ExpressionMenuBuilder(VRCExpressionsMenu menu)
        {
            this.menu = menu;
        }

        /// <summary>
        /// Change the current containing menu to the specified one
        /// </summary>
        /// <param name="menu">The new menu</param>
        public void SetMenu(VRCExpressionsMenu menu)
        {
            this.menu = menu;
        }

        /// <summary>
        /// Returns the containing VRCExpressionMenu
        /// </summary>
        /// <returns>The containing VRCExpressionMenu</returns>
        public VRCExpressionsMenu GetMenu()
        {
            return menu;
        }

        /// <summary>
        /// Create an asset of the containing menu at the specified path
        /// </summary>
        /// <param name="path">Asset path</param>
        /// <returns>This ExpressionMenuBuilder for further operations</returns>
        public ExpressionMenuBuilder CreateAsset(string path)
        {
            AssetDatabase.CreateAsset(menu, path);
            return this;
        }

        /// <summary>
        /// Adds a button
        /// </summary>
        /// <param name="name">Display name</param>
        /// <param name="parameter">The synced parameter to be controlled</param>
        /// <param name="value">The value to be set to the synced parameter</param>
        /// <returns>This ExpressionMenuBuilder for further operations</returns>
        public ExpressionMenuBuilder AddButton(string name, string parameter, float value)
        {
            if (menu.controls.Count >= 8)
            {
                throw new MenuOverflowException(name, menu.controls.Count);
            }

            menu.controls.Add(new VRCExpressionsMenu.Control()
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.Button,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameter },
                value = value
            });
            return this;
        }

        public ExpressionMenuBuilder AddToggle(string name, string parameter, float value, Texture2D icon = null)
        {
            if (menu.controls.Count >= 8)
            {
                throw new MenuOverflowException(name, menu.controls.Count);
            }

            menu.controls.Add(new VRCExpressionsMenu.Control()
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameter },
                value = value,
                icon = icon
            });
            return this;
        }

        /// <summary>
        /// Begins a new sub-menu, used together with <code>EndNewSubMenu()</code>
        /// </summary>
        /// <param name="name">Display name</param>
        /// <param name="parameterOnOpen">The synced parameter to be controlled when it is opened</param>
        /// <param name="valueOnOpen">The value to be set to the synced parameter when it is opened</param>
        /// <returns>A new ExpressionMenuBuilder of the newly created sub-menu</returns>
        public ExpressionMenuBuilder BeginNewSubMenu(string name, Texture2D icon = null, string parameterOnOpen = null, float valueOnOpen = 1)
        {
            return new ExpressionMenuBuilder(this, name, parameterOnOpen, icon, valueOnOpen);
        }

        /// <summary>
        /// Ends the new sub-menu creation and returns to the parent builder, used together with <code>BeginNewSubMenu()</code>
        /// 
        /// It throws an exception if you did not use it with any <code>BeginNewSubMenu()</code> calls.
        /// </summary>
        /// <returns>The parent ExpressionMenuBuilder</returns>
        public ExpressionMenuBuilder EndNewSubMenu()
        {
            if (parentBuilder == null)
            {
                throw new System.Exception("There is no parent builder to end! Are you not using together with BeginNewSubMenu()?");
            }
            parentBuilder.AddSubMenu(parentName, menu, parentIcon, parentParameterOnOpen, parentValueOnOpen);
            return parentBuilder;
        }

        /// <summary>
        /// Adds a sub-menu
        /// </summary>
        /// <param name="name">Display name</param>
        /// <param name="subMenu">The sub-menu to be added</param>
        /// <param name="parameterOnOpen">The synced parameter to be controlled when it is opened</param>
        /// <param name="valueOnOpen">The value to be set to the synced parameter when it is opened</param>
        /// <returns>This ExpressionMenuBuilder for further operations</returns>
        public ExpressionMenuBuilder AddSubMenu(string name, VRCExpressionsMenu subMenu, Texture2D icon = null, string parameterOnOpen = null, float valueOnOpen = 1)
        {
            if (menu.controls.Count >= 8)
            {
                throw new MenuOverflowException(name, menu.controls.Count);
            }

            VRCExpressionsMenu.Control control = new VRCExpressionsMenu.Control()
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                subMenu = subMenu,
                icon = icon,
                parameter = new VRCExpressionsMenu.Control.Parameter()
                {
                    name = ""
                }
            };

            if (parameterOnOpen != null)
            {
                control.parameter = new VRCExpressionsMenu.Control.Parameter()
                {
                    name = parameterOnOpen
                };
                control.value = valueOnOpen;
            }

            menu.controls.Add(control);

            return this;
        }

        public ExpressionMenuBuilder AddTwoAxisPuppet(string name, string horizontalParameter, string verticalParameter, string horizontalLabel, string verticalLabel, string parameterOnOpen = null, float valueOnOpen = 1)
        {
            return AddTwoAxisPuppet(name, horizontalParameter, verticalParameter, new VRCExpressionsMenu.Control.Label { name = horizontalLabel }, new VRCExpressionsMenu.Control.Label { name = verticalLabel }, parameterOnOpen, valueOnOpen);
        }

        /// <summary>
        /// Adds a two-axis puppet
        /// </summary>
        /// <param name="name">Display name</param>
        /// <param name="horizontalParameter">The synced parameter for the horizontal axis</param>
        /// <param name="verticalParameter">The synced parameter for the vertical axis</param>
        /// <param name="horizontalLabel">The label for the horizontal axis</param>
        /// <param name="verticalLabel">The label for the vertical axis</param>
        /// <param name="parameterOnOpen">The synced parameter to be controlled when it is opened</param>
        /// <param name="valueOnOpen">The value to be set to the synced parameter when it is opened</param>
        /// <returns>This ExpressionMenuBuilder for further operations</returns>
        public ExpressionMenuBuilder AddTwoAxisPuppet(string name, string horizontalParameter, string verticalParameter, VRCExpressionsMenu.Control.Label horizontalLabel, VRCExpressionsMenu.Control.Label verticalLabel, string parameterOnOpen = null, float valueOnOpen = 1)
        {
            if (menu.controls.Count >= 8)
            {
                throw new MenuOverflowException(name, menu.controls.Count);
            }

            VRCExpressionsMenu.Control control = new VRCExpressionsMenu.Control()
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet,
                subParameters = new VRCExpressionsMenu.Control.Parameter[]
                {
                    new VRCExpressionsMenu.Control.Parameter() { name = horizontalParameter },
                    new VRCExpressionsMenu.Control.Parameter() { name = verticalParameter }
                },
                labels = new VRCExpressionsMenu.Control.Label[]
                {
                    horizontalLabel,
                    verticalLabel
                }
            };

            if (parameterOnOpen != null)
            {
                control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterOnOpen };
                control.value = valueOnOpen;
            }

            menu.controls.Add(control);

            return this;
        }

        public ExpressionMenuBuilder AddFourAxisPuppet(string name, string upParameter, string rightParameter, string downParameter, string leftParameter, string upLabel, string rightLabel, string downLabel, string leftLabel, string parameterOnOpen = null, float valueOnOpen = 1)
        {
            return AddFourAxisPuppet(name, upParameter, rightParameter, downParameter, leftParameter, new VRCExpressionsMenu.Control.Label { name = upLabel }, new VRCExpressionsMenu.Control.Label { name = rightLabel }, new VRCExpressionsMenu.Control.Label { name = downLabel }, new VRCExpressionsMenu.Control.Label { name = leftLabel }, parameterOnOpen, valueOnOpen);
        }

        /// <summary>
        /// Adds a four-axis puppet
        /// </summary>
        /// <param name="name">Display name</param>
        /// <param name="upParameter">The synced parameter for the up axis</param>
        /// <param name="rightParameter">The synced parameter for the right axis</param>
        /// <param name="downParameter">The synced parameter for the down axis</param>
        /// <param name="leftParameter">The synced parameter for the left axis</param>
        /// <param name="upLabel">The label for the up axis</param>
        /// <param name="rightLabel">The label for the right axis</param>
        /// <param name="downLabel">The label for the down axis</param>
        /// <param name="leftLabel">The label for the left axis</param>
        /// <param name="parameterOnOpen">The synced parameter to be controlled when it is opened</param>
        /// <param name="valueOnOpen">The value to be set to the synced parameter when it is opened</param>
        /// <returns>This ExpressionMenuBuilder for further operations</returns>
        public ExpressionMenuBuilder AddFourAxisPuppet(string name, string upParameter, string rightParameter, string downParameter, string leftParameter, VRCExpressionsMenu.Control.Label upLabel, VRCExpressionsMenu.Control.Label rightLabel, VRCExpressionsMenu.Control.Label downLabel, VRCExpressionsMenu.Control.Label leftLabel, string parameterOnOpen = null, float valueOnOpen = 1)
        {
            if (menu.controls.Count >= 8)
            {
                throw new MenuOverflowException(name, menu.controls.Count);
            }

            VRCExpressionsMenu.Control control = new VRCExpressionsMenu.Control()
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.FourAxisPuppet,
                subParameters = new VRCExpressionsMenu.Control.Parameter[]
                {
                    new VRCExpressionsMenu.Control.Parameter() { name = upParameter },
                    new VRCExpressionsMenu.Control.Parameter() { name = rightParameter },
                    new VRCExpressionsMenu.Control.Parameter() { name = downParameter },
                    new VRCExpressionsMenu.Control.Parameter() { name = leftParameter }
                },
                labels = new VRCExpressionsMenu.Control.Label[]
                {
                    upLabel,
                    rightLabel,
                    downLabel,
                    leftLabel
                }
            };

            if (parameterOnOpen != null)
            {
                control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterOnOpen };
                control.value = valueOnOpen;
            }

            menu.controls.Add(control);

            return this;
        }

        /// <summary>
        /// Adds a radial puppet
        /// </summary>
        /// <param name="name">Display name</param>
        /// <param name="rotationParameter">The synced parameter for the rotation</param>
        /// <param name="parameterOnOpen">The synced parameter to be controlled when it is opened</param>
        /// <param name="valueOnOpen">The value to be set to the synced parameter when it is opened</param>
        /// <returns>This ExpressionMenuBuilder for further operations</returns>
        public ExpressionMenuBuilder AddRadialPuppet(string name, string rotationParameter, string parameterOnOpen = null, float valueOnOpen = 1)
        {
            if (menu.controls.Count >= 8)
            {
                throw new MenuOverflowException(name, menu.controls.Count);
            }

            VRCExpressionsMenu.Control control = new VRCExpressionsMenu.Control()
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                subParameters = new VRCExpressionsMenu.Control.Parameter[]
                {
                    new VRCExpressionsMenu.Control.Parameter() { name = rotationParameter }
                }
            };

            if (parameterOnOpen != null)
            {
                control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterOnOpen };
                control.value = valueOnOpen;
            }

            menu.controls.Add(control);

            return this;
        }
    }
}
#endif

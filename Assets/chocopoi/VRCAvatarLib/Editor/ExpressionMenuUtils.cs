using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.VRCAvatarLib
{
    public class ExpressionMenuUtils
    {
        public static ExpressionMenuBuilder CreateSubMenu(VRCExpressionsMenu rootMenu, string controlName)
        {
            // create builder
            ExpressionMenuBuilder builder = new ExpressionMenuBuilder();
            AddSubMenu(rootMenu, controlName, builder.GetMenu());
            return builder;
        }

        public static void CreateSubMenu(VRCExpressionsMenu rootMenu, string controlName, List<VRCExpressionsMenu.Control> controls)
        {
            // create menu

            VRCExpressionsMenu menu = (VRCExpressionsMenu)ScriptableObject.CreateInstance(typeof(VRCExpressionsMenu));
            menu.controls = controls;

            AddSubMenu(rootMenu, controlName, menu);
        }

        public static void AddSubMenu(VRCExpressionsMenu rootMenu, string controlName, VRCExpressionsMenu subMenu)
        {
            // return if count larger or equal to 8

            if (rootMenu.controls.Count >= 8)
            {
                return;
            }

            // create menu

            rootMenu.controls.Add(new VRCExpressionsMenu.Control()
            {
                name = controlName,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                subMenu = subMenu
            });
        }

        public static void RemoveExpressionMenu(VRCExpressionsMenu menu, string regex)
        {
            RemoveExpressionMenu(menu, new Regex(regex));
        }

        public static void RemoveExpressionMenu(VRCExpressionsMenu menu, Regex regex)
        {
            // clean up menu

            foreach (VRCExpressionsMenu.Control control in menu.controls)
            {
                if (regex.IsMatch(control.name))
                {
                    menu.controls.Remove(control);
                }
            }

            // save to file

            AssetDatabase.SaveAssets();
        }

        public static void RemoveExpressionParameters(VRCExpressionParameters parameters, string regex)
        {
            RemoveExpressionParameters(parameters, new Regex(regex));
        }

        public static void RemoveExpressionParameters(VRCExpressionParameters parameters, Regex regex)
        {
            // clean up parameters

            List<VRCExpressionParameters.Parameter> list = new List<VRCExpressionParameters.Parameter>(parameters.parameters);
            List<VRCExpressionParameters.Parameter> toDelete = new List<VRCExpressionParameters.Parameter>();

            foreach (VRCExpressionParameters.Parameter p in list)
            {
                if (regex.IsMatch(p.name))
                {
                    toDelete.Add(p);
                }
            }

            foreach (VRCExpressionParameters.Parameter p in toDelete)
            {
                list.Remove(p);
            }

            parameters.parameters = list.ToArray();

            // save to file

            AssetDatabase.SaveAssets();
        }
    }
}
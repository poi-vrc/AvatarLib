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
    /// An utility class to make modification to an VRCExpressionMenu
    /// </summary>
    public class ExpressionMenuUtils
    {
        /// <summary>
        /// Creates a sub-menu in the specified root menu using a ExpressionMenuBuilder
        /// </summary>
        /// <param name="rootMenu">The root menu to add a sub-menu</param>
        /// <param name="controlName">The sub-menu display name</param>
        /// <returns>A ExpressionMenuBuilder for adding controls to the new menu</returns>
        public static ExpressionMenuBuilder CreateSubMenu(VRCExpressionsMenu rootMenu, string controlName)
        {
            // create builder
            ExpressionMenuBuilder builder = new ExpressionMenuBuilder();
            AddSubMenu(rootMenu, controlName, builder.GetMenu());
            return builder;
        }

        /// <summary>
        /// Creates a sub-menu in the specified root menu using a list of VRCExpressionMenu.Control
        /// </summary>
        /// <param name="rootMenu">The root menu to add a sub-menu</param>
        /// <param name="controlName">The sub-menu display name</param>
        /// <param name="controls">The controls to be added to the new sub-menu</param>
        public static void CreateSubMenu(VRCExpressionsMenu rootMenu, string controlName, List<VRCExpressionsMenu.Control> controls)
        {
            // create menu

            VRCExpressionsMenu menu = (VRCExpressionsMenu)ScriptableObject.CreateInstance(typeof(VRCExpressionsMenu));
            menu.controls = controls;

            AddSubMenu(rootMenu, controlName, menu);
        }

        /// <summary>
        /// Adds a sub-menu to the specified root menu. Throws a MenuOverflowException if the specified menu is already full (8 controls).
        /// </summary>
        /// <param name="rootMenu">The root menu to add a sub-menu</param>
        /// <param name="controlName">The sub-menu display name</param>
        /// <param name="subMenu">The sub-menu to be added</param>
        public static void AddSubMenu(VRCExpressionsMenu rootMenu, string controlName, VRCExpressionsMenu subMenu)
        {
            // return if count larger or equal to 8

            if (rootMenu.controls.Count >= 8)
            {
                throw new MenuOverflowException(controlName, rootMenu.controls.Count);
            }

            // create menu

            rootMenu.controls.Add(new VRCExpressionsMenu.Control()
            {
                name = controlName,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                subMenu = subMenu
            });
        }

        /// <summary>
        /// Remove controls that matches with the Regex pattern from the specified menu.
        /// 
        /// If you use this multiple times with the same regex, it is better to use a Regex
        /// instance and pass it to this function. Because this will create a new regex instance
        /// everytime you call.
        /// 
        /// Caution: This does not save to file. Execute <code>AssetDatabase.SaveAssets()</code> to save.
        /// </summary>
        /// <param name="menu">The menu to be modified</param>
        /// <param name="regex">A Regex string</param>
        public static void RemoveExpressionMenuControls(VRCExpressionsMenu menu, string regex)
        {
            RemoveExpressionMenuControls(menu, new Regex(regex));
        }

        /// <summary>
        /// Remove controls that matches with the Regex pattern from the specified menu
        /// 
        /// Caution: This does not save to file. Execute <code>AssetDatabase.SaveAssets()</code> to save.
        /// </summary>
        /// <param name="menu">The menu to be modified</param>
        /// <param name="regex">A Regex instance</param>
        public static void RemoveExpressionMenuControls(VRCExpressionsMenu menu, Regex regex)
        {
            // clean up menu
            
            List<VRCExpressionsMenu.Control> toDelete = new List<VRCExpressionsMenu.Control>();

            foreach (VRCExpressionsMenu.Control p in menu.controls)
            {
                if (regex.IsMatch(p.name))
                {
                    toDelete.Add(p);
                }
            }

            foreach (VRCExpressionsMenu.Control p in toDelete)
            {
                menu.controls.Remove(p);
            }
        }

        /// <summary>
        /// Remove expression parameters that matches with the Regex specified.
        /// 
        /// If you use this multiple times with the same regex, it is better to use a Regex
        /// instance and pass it to this function. Because this will create a new regex instance
        /// everytime you call.
        /// 
        /// Caution: This does not save to file. Execute <code>AssetDatabase.SaveAssets()</code> to save.
        /// </summary>
        /// <param name="parameters">The parameters to be removed from</param>
        /// <param name="regex">A Regex string</param>
        public static void RemoveExpressionParameters(VRCExpressionParameters parameters, string regex)
        {
            RemoveExpressionParameters(parameters, new Regex(regex));
        }

        /// <summary>
        /// Remove expression parameters that matches with the Regex specified.
        /// 
        /// Caution: This does not save to file. Execute <code>AssetDatabase.SaveAssets()</code> to save.
        /// </summary>
        /// <param name="parameters">The parameters to be removed from</param>
        /// <param name="regex">A Regex instance</param>
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
        }

        /// <summary>
        /// Calculates the total parameters cost from the specified parameters
        /// </summary>
        /// <param name="parameters">Parameters to calculate</param>
        /// <returns>The cost</returns>
        public static int CalculateParametersCost(IEnumerable<VRCExpressionParameters.Parameter> parameters)
        {
            int cost = 0;

            foreach (VRCExpressionParameters.Parameter p in parameters)
            {
                cost += VRCExpressionParameters.TypeCost(p.valueType);
            }

            return cost;
        }

        /// <summary>
        /// Calculates the total parameters remain
        /// </summary>
        /// <param name="parameters">The existing parameters</param>
        /// <param name="newParameters">The new parameters to be added</param>
        /// <returns></returns>
        public static int CalculateParametersRemain(VRCExpressionParameters parameters, IEnumerable<VRCExpressionParameters.Parameter> newParameters)
        {
            return VRCExpressionParameters.MAX_PARAMETER_COST - parameters.CalcTotalCost() - CalculateParametersCost(newParameters);
        }

        /// <summary>
        /// Adds the new parameters, if the cost exceeds VRChat limits, it throws a ParameterOverflowException
        /// </summary>
        /// <param name="parameters">The existing parameters</param>
        /// <param name="newParameters">The new parameters to be added</param>
        public static void AddExpressionParameters(VRCExpressionParameters parameters, IEnumerable<VRCExpressionParameters.Parameter> newParameters)
        {
            int cost = CalculateParametersCost(newParameters);
            int ogCost = parameters.CalcTotalCost();

            if (VRCExpressionParameters.MAX_PARAMETER_COST - ogCost - cost < 0)
            {
                throw new ParameterOverflowException(ogCost, ogCost + cost, VRCExpressionParameters.MAX_PARAMETER_COST);
            }

            List<VRCExpressionParameters.Parameter> list = new List<VRCExpressionParameters.Parameter>(parameters.parameters);

            foreach (VRCExpressionParameters.Parameter p in newParameters)
            {
                list.Add(p);
            }

            parameters.parameters = list.ToArray();
        }
    }
}
#endif

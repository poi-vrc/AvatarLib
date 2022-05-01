using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.AvatarLib
{
    public class ExpressionMenuBuilder
    {
        private VRCExpressionsMenu menu;

        public ExpressionMenuBuilder()
        {
            menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
        }

        public ExpressionMenuBuilder(VRCExpressionsMenu menu)
        {
            this.menu = menu;
        }

        public VRCExpressionsMenu GetMenu()
        {
            return menu;
        }

        public ExpressionMenuBuilder AddSubMenu(string name, VRCExpressionsMenu subMenu)
        {
            menu.controls.Add(new VRCExpressionsMenu.Control()
            {

            });
            return this;
        }
    }
}
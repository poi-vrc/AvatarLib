using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chocopoi.AvatarLib.Expressions;
using Chocopoi.AvatarLib.WorldConstraint;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ExpressionMenuUtils.CreateSubMenu(null, "test")
            .AddButton("", "", -1)
            .AddToggle("test", "", -1)
            .BeginNewSubMenu("AnotherMenu")
                .AddToggle("test", "test", 1)
                .CreateAsset("")
            .EndNewSubMenu()
            .CreateAsset("")
            .GetMenu();

        ExpressionMenuUtils.CalculateParametersCost(new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter[] { });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

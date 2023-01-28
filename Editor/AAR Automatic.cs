using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Animations;


namespace AutoAnimationRepath
{
    [InitializeOnLoad]
    public static class AARAutomatic
    {
        //Fuck you why is OnHierarchyChange() only for editor windows that doesnt even make fucking sense why do I have to do this subscribe workaround bs
        static AARAutomatic()
        {
            EditorApplication.hierarchyChanged += HierarchyChange;
        }
        static void HierarchyChange()
        {
            EditorApplication.Beep();
            Debug.Log("Hierarchy Change");
        }
    }
}
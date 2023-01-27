using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.Animations;
using UnityEngine.Animations;
using System.Text.RegularExpressions;

namespace AutoRepathing
{

    public class AutoRepathingWindow : EditorWindow
    {
        bool Active;
        string NewPath;
        Transform Selected;
        string NewName;
        string NewNameProxy;
        Transform RootParent;
        Transform PreviousParent;
        string PreviousName;
        Animator TargetAnimator;
        RuntimeAnimatorController RuntimeController;
        AnimatorController TargetController;
        

        int ClipCount = 0;
        int PropertyCount = 0;
        int RepathedCount = 0;

        [MenuItem("hfcRed/Auto Repathing")]
        public static void ShowWindow()
        {
            AutoRepathingWindow Window = GetWindow<AutoRepathingWindow>(false, "Auto Repathing", true);
            Window.minSize = new Vector2(40, 30);
        }

        void OnGUI()
        {
            {
                Active = EditorGUILayout.Toggle("Active", Active);
            }
        }

        void OnSelectionChange()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }
            PreviousParent = Selection.activeTransform.parent;

            PreviousName = Selection.activeGameObject.name;
            if (PreviousName == null)
            {
                return;
            }
        }

        void OnHierarchyChange()
        {
            if (Active == true)
            {
                if (Selection.activeGameObject == null)
                {
                    return;
                }
                NewName = Selection.activeGameObject.name;
                Selected = Selection.activeTransform;
                if (Selected.parent == null)
                {
                    return;
                }

                if (PreviousParent != Selected.parent && EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties for " + Selected.name + " to " + Selected.parent.name, "Continue", "Cancel"))
                {
                    Repath();
                }

                if (PreviousName != null && PreviousName != NewName && EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties from " + PreviousName + " to " + NewName, "Continue", "Cancel"))
                {
                    if (NewName.Contains(PreviousName))
                    {
                        Debug.LogWarning("New name contains old name, creating proxy");
                        NewNameProxy = NewName;
                        NewName = "Proxy";
                        Rename();
                        NewName = NewNameProxy;
                        Debug.LogWarning("Finished creating proxy, renaming to new name");
                    }

                    Rename();
                }
            }
        }

        public void Repath()
        {
            RootParent = Selected.transform.root;
            TargetAnimator = RootParent.GetComponent<Animator>();
            RuntimeController = TargetAnimator.runtimeAnimatorController;
            TargetController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(RuntimeController));

            NewPath = AnimationUtility.CalculateTransformPath(Selected.parent, RootParent);
            if (Selected.parent != RootParent)
            {
                NewPath = AnimationUtility.CalculateTransformPath(Selected.parent, RootParent) + "/";
            }


            AssetDatabase.StartAssetEditing();

            var Timer = System.Diagnostics.Stopwatch.StartNew();

            foreach (AnimationClip Clip in TargetController.animationClips)
            {
                ClipCount++;
                Array Curves = AnimationUtility.GetCurveBindings(Clip);

                foreach (EditorCurveBinding X in Curves)
                {
                    PropertyCount++;
                    EditorCurveBinding Binding = X;
                    AnimationCurve Curve = AnimationUtility.GetEditorCurve(Clip, Binding);
               
                    if (Binding.path.Contains(Selected.name))
                    {
                        RepathedCount++;
                        AnimationUtility.SetEditorCurve(Clip, Binding, null);                        
                        string OldPath = Binding.path.Substring(Binding.path.IndexOf(Selected.name));
                        string FullPath = NewPath + OldPath;
                        Binding.path = FullPath;
                        AnimationUtility.SetEditorCurve(Clip, Binding, Curve);
                    }                    
                }
            }
            AssetDatabase.StopAssetEditing();
            PreviousParent = Selection.activeTransform.parent;

            Debug.Log("Clips Checked: " + ClipCount);
            Debug.Log("Properties Checked: " + PropertyCount);
            Debug.Log("Properties Repathed: " + RepathedCount);
            ClipCount = 0;
            PropertyCount = 0;
            RepathedCount = 0;

            Timer.Stop();
            var Time = Timer.ElapsedMilliseconds;
            Debug.Log("Execution Time: " + Time + " ms");
            Timer.Reset();
        }


        public void Rename()
        {
            RootParent = Selected.transform.root;
            TargetAnimator = RootParent.GetComponent<Animator>();
            RuntimeController = TargetAnimator.runtimeAnimatorController;
            TargetController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(RuntimeController));

            AssetDatabase.StartAssetEditing();

            var Timer = System.Diagnostics.Stopwatch.StartNew();

            foreach (AnimationClip Clip in TargetController.animationClips)
            {
                ClipCount++;
                Array Curves = AnimationUtility.GetCurveBindings(Clip);

                foreach (EditorCurveBinding X in Curves)
                {
                    PropertyCount++;
                    EditorCurveBinding Binding = X;
                    AnimationCurve Curve = AnimationUtility.GetEditorCurve(Clip, Binding);

                    

                    if (Binding.path.Contains(PreviousName))
                    {
                        AnimationUtility.SetEditorCurve(Clip, Binding, null);
                        RepathedCount++;
                        string Path = Binding.path.Replace(PreviousName, NewName);
                        Binding.path = Path;
                        AnimationUtility.SetEditorCurve(Clip, Binding, Curve);
                    }
                    
                }
            }
            AssetDatabase.StopAssetEditing();
            PreviousName = NewName;

            Debug.Log("Clips Checked: " + ClipCount);
            Debug.Log("Properties Checked: " + PropertyCount);
            Debug.Log("Properties Renamed: " + RepathedCount);
            ClipCount = 0;
            PropertyCount = 0;
            RepathedCount = 0;

            Timer.Stop();
            var Time = Timer.ElapsedMilliseconds;
            Debug.Log("Execution Time: " + Time + " ms");
            Timer.Reset();
        }
    }  
}
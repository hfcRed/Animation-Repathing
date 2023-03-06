using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Animations;


namespace AutoAnimationRepath
{
    [InitializeOnLoad]
    public class AARManual
    {
        public static List<List<AnimationClip>> invalidClips = new List<List<AnimationClip>>();
        public static List<string> invalidPaths = new List<string>();
        public static List<bool> foldouts = new List<bool>();
        public static List<string> newPaths = new List<string>();

        public static void ScanPaths()
        {
            Transform parent = Selection.activeTransform.root;
            GameObject parentObject = parent.gameObject;
            Animator parentAnimator = parent.GetComponent<Animator>();
            RuntimeAnimatorController runtime = parentAnimator.runtimeAnimatorController;
            AARAutomatic.controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(runtime));

            invalidClips.Clear();
            invalidPaths.Clear();
            foldouts.Clear();
            newPaths.Clear();

            List<AnimationClip> invalidClip = new List<AnimationClip>();
            object animatedObject = 0;

            foreach (AnimationClip clip in AARAutomatic.controller.animationClips)
            {
                Array curves = AnimationUtility.GetCurveBindings(clip);

                foreach (EditorCurveBinding x in curves)
                {
                    EditorCurveBinding curve = x;
                    animatedObject = AnimationUtility.GetAnimatedObject(parentObject, curve);

                    if (animatedObject == null && !invalidPaths.Contains(curve.path))
                    {
                        newPaths.Add(string.Empty);
                        foldouts.Add(new bool());
                        invalidPaths.Add(curve.path);
                        invalidClips.Add(new List<AnimationClip>());
                    }
                    if (animatedObject == null && !invalidClips[invalidPaths.IndexOf(curve.path)].Contains(clip))
                    {
                        invalidClips[invalidPaths.IndexOf(curve.path)].Add(clip);
                    }
                }
                animatedObject = 0;
            }
            AARAutomatic.foldout = true;
        }

        public static void RenamePath()
        {

        }
    }
}
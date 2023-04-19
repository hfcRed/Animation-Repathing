using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using static AutoAnimationRepath.AARVariables;
using System.Linq;

namespace AutoAnimationRepath
{
    [InitializeOnLoad]
    public class AARManual
    {
        static AARManual()
        {
            Selection.selectionChanged += ClipEditing.GetClipPaths;
        }

        public static class InvalidPaths
        {
            public static void ScanInvalidPaths()
            {
                invalidClips.Clear();
                invalidOldPaths.Clear();
                invalidFoldouts.Clear();
                invalidNewPaths.Clear();

                List<AnimationClip> invalidClip = new List<AnimationClip>();
                object animatedObject = 0;

                foreach (AnimationClip clip in controller.animationClips)
                {
                    Array curves = AnimationUtility.GetCurveBindings(clip);

                    foreach (EditorCurveBinding x in curves)
                    {
                        EditorCurveBinding curve = x;
                        animatedObject = AnimationUtility.GetAnimatedObject(AARAutomatic.GetRoot().gameObject, curve);

                        if (animatedObject == null && !invalidOldPaths.Contains(curve.path))
                        {
                            invalidFoldouts.Add(new bool());
                            invalidOldPaths.Add(curve.path);
                            invalidClips.Add(new List<AnimationClip>());
                            invalidNewPaths.Add(curve.path);

                        }
                        if (animatedObject == null && !invalidClips[invalidOldPaths.IndexOf(curve.path)].Contains(clip))
                        {
                            invalidClips[invalidOldPaths.IndexOf(curve.path)].Add(clip);
                        }
                    }
                    animatedObject = 0;
                }
                invalidPathsFoldout = true;
            }

            public static void RenameInvalidPaths()
            {
                GameObject parentObject = animator.gameObject;
                object animatedObject = 0;

                try
                {
                    AssetDatabase.StartAssetEditing();

                    foreach (AnimationClip clip in invalidClips[invalidPosition])
                    {
                        Array curves = AnimationUtility.GetCurveBindings(clip);

                        foreach (EditorCurveBinding x in curves)
                        {
                            EditorCurveBinding binding = x;
                            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                            animatedObject = AnimationUtility.GetAnimatedObject(parentObject, binding);

                            if (animatedObject == null)
                            {
                                AnimationUtility.SetEditorCurve(clip, binding, null);
                                binding.path = invalidNewPaths[invalidPosition];
                                AnimationUtility.SetEditorCurve(clip, binding, curve);
                            }
                        }
                        animatedObject = 0;
                    }
                }
                finally { AssetDatabase.StopAssetEditing(); }

                invalidNewPaths.RemoveAt(invalidPosition);
                invalidOldPaths.RemoveAt(invalidPosition);
                invalidFoldouts.RemoveAt(invalidPosition);
                invalidClips.RemoveAt(invalidPosition);
            }
        }

        public static class ClipEditing
        {
            public static void GetClipPaths()
            {
                clipsClips.Clear();
                sharedProperties.Clear();
                pathToSharedProperty.Clear();

                clipsClips = Selection.GetFiltered<AnimationClip>(SelectionMode.Assets).ToList();

                foreach (AnimationClip clip in clipsClips)
                {
                    Array curves = AnimationUtility.GetCurveBindings(clip);

                    foreach (EditorCurveBinding curve in curves)
                    {
                        if (pathToSharedProperty.TryGetValue(curve.path, out SharedProperty sp))
                        {
                            if (!sp.foldoutClips.Contains(clip))
                            {
                                sp.foldoutClips.Add(clip);                                
                            }
                            sp.count++;
                        }
                        else
                        {
                            SharedProperty sp2 = new SharedProperty();
                            pathToSharedProperty.Add(curve.path, sp2);
                            sp2.oldPath = curve.path;
                            sp2.newPath = curve.path;
                            sp2.foldoutClips.Add(clip);
                            sp2.count++;
                        }
                    }
                }
            }

            public static void RenameClipPaths(AnimationClip clip, bool replaceEntire, string oldPath, string newPath)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();

                    Array curves = AnimationUtility.GetCurveBindings(clip);

                    foreach (EditorCurveBinding x in curves)
                    {
                        EditorCurveBinding binding = x;
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                        if (replaceEntire == false && binding.path.Contains(clipsReplaceFrom))
                        {
                            AnimationUtility.SetEditorCurve(clip, binding, null);
                            binding.path = binding.path.Replace(clipsReplaceFrom, clipsReplaceTo);
                            AnimationUtility.SetEditorCurve(clip, binding, curve);
                        }

                        if (replaceEntire == true && binding.path == oldPath)
                        {
                            AnimationUtility.SetEditorCurve(clip, binding, null);
                            binding.path = newPath;
                            AnimationUtility.SetEditorCurve(clip, binding, curve);
                        }
                    }
                }
                finally { AssetDatabase.StopAssetEditing(); GetClipPaths(); }
            }

        }
    }
}
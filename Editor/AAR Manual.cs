using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static AutoAnimationRepath.AARVariables;

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
                invalidSharedProperties.Clear();
                invalidPathToSharedProperty.Clear();

                foreach (AnimationClip clip in controller.animationClips)
                {
                    Array curves = AnimationUtility.GetCurveBindings(clip);

                    foreach (EditorCurveBinding curve in curves)
                    {
                        object animatedObject = 0;
                        animatedObject = AnimationUtility.GetAnimatedObject(animator.gameObject, curve);

                        if (animatedObject == null && invalidPathToSharedProperty.TryGetValue(curve.path, out InvalidSharedProperty sp))
                        {
                            if (!sp.foldoutClips.Contains(clip))
                            {
                                sp.foldoutClips.Add(clip);
                            }
                            sp.count++;
                        }
                        else if (animatedObject == null)
                        {
                            InvalidSharedProperty sp2 = new InvalidSharedProperty();
                            invalidPathToSharedProperty.Add(curve.path, sp2);
                            sp2.oldPath = curve.path;
                            sp2.newPath = curve.path;
                            sp2.foldoutClips.Add(clip);
                            sp2.count++;
                        }
                    }
                }
            }

            public static void RenameInvalidPaths(AnimationClip clip, string oldPath, string newPath)
            {
                AssetDatabase.StartAssetEditing();

                Array curves = AnimationUtility.GetCurveBindings(clip);

                foreach (EditorCurveBinding x in curves)
                {
                    object animatedObject = 0;
                    EditorCurveBinding binding = x;
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                    animatedObject = AnimationUtility.GetAnimatedObject(AARAutomatic.GetRoot().gameObject, binding);

                    if (animatedObject == null && binding.path.Contains(oldPath))
                    {
                        AnimationUtility.SetEditorCurve(clip, binding, null);
                        binding.path = newPath;
                        AnimationUtility.SetEditorCurve(clip, binding, curve);
                    }
                }
            }
        }

        public static class ClipEditing
        {
            public static void GetClipPaths()
            {
                clipsSelected.Clear();
                clipsSharedProperties.Clear();
                clipsPathToSharedProperty.Clear();

                clipsSelected = Selection.GetFiltered<AnimationClip>(SelectionMode.Assets).ToList();

                foreach (AnimationClip clip in clipsSelected)
                {
                    Array curves = AnimationUtility.GetCurveBindings(clip);

                    foreach (EditorCurveBinding curve in curves)
                    {
                        if (clipsPathToSharedProperty.TryGetValue(curve.path, out ClipsSharedProperty sp))
                        {
                            if (!sp.foldoutClips.Contains(clip))
                            {
                                sp.foldoutClips.Add(clip);
                            }
                            sp.count++;
                        }
                        else
                        {
                            ClipsSharedProperty sp2 = new ClipsSharedProperty();
                            clipsPathToSharedProperty.Add(curve.path, sp2);
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
                Array curves = AnimationUtility.GetCurveBindings(clip);

                foreach (EditorCurveBinding x in curves)
                {
                    EditorCurveBinding binding = x;
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                    if (replaceEntire == false && binding.path.Contains(oldPath))
                    {
                        AnimationUtility.SetEditorCurve(clip, binding, null);
                        binding.path = binding.path.Replace(oldPath, newPath);
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
        }
    }
}
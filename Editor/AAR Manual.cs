using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
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
            public static void ScanInvalidPaths(AnimatorController controller)
            {
                invalidSharedProperties.Clear();
                invalidPathToSharedProperty.Clear();

                foreach (AnimationClip clip in controller.animationClips)
                {
                    Array curves = AnimationUtility.GetCurveBindings(clip);

                    foreach (EditorCurveBinding curve in curves)
                    {
                        object animatedObject;
                        animatedObject = GetRoot() == null ? (object)0 : AnimationUtility.GetAnimatedObject(GetRoot().gameObject, curve);

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
                EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                foreach (EditorCurveBinding x in floatCurves)
                {
                    object animatedObject;
                    EditorCurveBinding binding = x;
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                    animatedObject = GetRoot() == null ? (object)0 : AnimationUtility.GetAnimatedObject(GetRoot().gameObject, binding);

                    if (animatedObject == null && binding.path.Contains(oldPath))
                    {
                        AnimationUtility.SetEditorCurve(clip, binding, null);
                        binding.path = newPath;
                        AnimationUtility.SetEditorCurve(clip, binding, curve);
                    }
                }

                foreach (EditorCurveBinding x in objectCurves)
                {
                    object animatedObject;
                    EditorCurveBinding binding = x;
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                    animatedObject = GetRoot() == null ? (object)0 : AnimationUtility.GetAnimatedObject(GetRoot().gameObject, binding);

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
                    EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                    EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                    foreach (EditorCurveBinding curve in floatCurves)
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

                    foreach (EditorCurveBinding curve in objectCurves)
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
                EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                foreach (EditorCurveBinding x in floatCurves)
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

                foreach (EditorCurveBinding x in objectCurves)
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
using static AutoAnimationRepath.ARVariables;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AutoAnimationRepath
{
    [InitializeOnLoad]
    public class ARManual
    {
        static ARManual()
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

                foreach (EditorCurveBinding binding in floatCurves) ChangeBindings(binding, false);
                foreach (EditorCurveBinding binding in objectCurves) ChangeBindings(binding, true);

                void ChangeBindings(EditorCurveBinding binding, bool isObjectCurve)
                {
                    object animatedObject;
                    animatedObject = GetRoot() == null ? (object)0 : AnimationUtility.GetAnimatedObject(GetRoot().gameObject, binding);
                    if (isObjectCurve && binding.path == oldPath)
                    {
                        ObjectReferenceKeyframe[] objectCurve = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                        binding.path = newPath;
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, objectCurve);
                    }
                    else if (binding.path == oldPath)
                    {
                        AnimationCurve floatCurve = AnimationUtility.GetEditorCurve(clip, binding);
                        AnimationUtility.SetEditorCurve(clip, binding, null);
                        binding.path = newPath;
                        AnimationUtility.SetEditorCurve(clip, binding, floatCurve);
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

                foreach (EditorCurveBinding binding in floatCurves) ChangeBindings(binding, false);
                foreach (EditorCurveBinding binding in objectCurves) ChangeBindings(binding, true);

                void ChangeBindings(EditorCurveBinding binding, bool isObjectCurve)
                {
                    if (isObjectCurve)
                    {
                        ObjectReferenceKeyframe[] objectCurve = AnimationUtility.GetObjectReferenceCurve(clip, binding);

                        if (!replaceEntire && binding.path.Contains(oldPath))
                        {
                            AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                            binding.path = binding.path.Replace(oldPath, newPath);
                            AnimationUtility.SetObjectReferenceCurve(clip, binding, objectCurve);
                        }

                        if (replaceEntire && binding.path == oldPath)
                        {
                            AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                            binding.path = newPath;
                            AnimationUtility.SetObjectReferenceCurve(clip, binding, objectCurve);
                        }
                    }
                    else
                    {
                        AnimationCurve floatCurve = AnimationUtility.GetEditorCurve(clip, binding);

                        if (!replaceEntire && binding.path.Contains(oldPath))
                        {
                            AnimationUtility.SetEditorCurve(clip, binding, null);
                            binding.path = binding.path.Replace(oldPath, newPath);
                            AnimationUtility.SetEditorCurve(clip, binding, floatCurve);
                        }

                        if (replaceEntire && binding.path == oldPath)
                        {
                            AnimationUtility.SetEditorCurve(clip, binding, null);
                            binding.path = newPath;
                            AnimationUtility.SetEditorCurve(clip, binding, floatCurve);
                        }
                    }
                }
            }
        }
    }
}
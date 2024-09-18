using static AnimationRepathing.ARVariables;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimationRepathing
{
    [InitializeOnLoad]
    public class ARManual
    {
        static ARManual()
        {
            Selection.selectionChanged -= ClipEditing.GetClipPaths;
            Selection.selectionChanged += ClipEditing.GetClipPaths;
        }

        public static class InvalidPaths
        {
            public static void ScanInvalidPaths(AnimatorController[] controllers)
            {
                invalidSharedProperties.Clear();
                invalidPathToSharedProperty.Clear();

                foreach (AnimatorController controller in controllers)
                {
                    foreach (AnimationClip clip in controller.animationClips)
                    {
                        EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                        EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                        foreach (EditorCurveBinding binding in floatCurves) GetValues(binding);
                        foreach (EditorCurveBinding binding in objectCurves) GetValues(binding);

                        void GetValues(EditorCurveBinding binding)
                        {
                            object animatedObject;
                            animatedObject = GetRoot() == null ? (object)0 : AnimationUtility.GetAnimatedObject(GetRoot().gameObject, binding);

                            if (animatedObject == null && invalidPathToSharedProperty.TryGetValue(binding.path, out InvalidSharedProperty sp))
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
                                invalidPathToSharedProperty.Add(binding.path, sp2);
                                sp2.oldPath = binding.path;
                                sp2.newPath = binding.path;
                                sp2.foldoutClips.Add(clip);
                                sp2.count++;
                            }
                        }
                    }
                }
            }

            public static void RenameInvalidPaths(AnimationClip[] clips, string oldPath, string newPath)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();

                    foreach (AnimationClip clip in clips)
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

                                PlayerPrefs.SetInt("ARRepathCount", PlayerPrefs.GetInt("ARRepathCount") + 1);
                            }
                            else if (binding.path == oldPath)
                            {
                                AnimationCurve floatCurve = AnimationUtility.GetEditorCurve(clip, binding);
                                AnimationUtility.SetEditorCurve(clip, binding, null);
                                binding.path = newPath;
                                AnimationUtility.SetEditorCurve(clip, binding, floatCurve);

                                PlayerPrefs.SetInt("ARRepathCount", PlayerPrefs.GetInt("ARRepathCount") + 1);
                            }
                        }
                    }
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                    ScanInvalidPaths(GetControllers().ToArray());
                    ARAutomatic.HierarchyChanged();
                }
            }
        }

        public static class ClipEditing
        {
            public static void GetClipPaths()
            {
                if (manualToolSelection == 1)
                {
                    InvalidPaths.ScanInvalidPaths(GetControllers().ToArray());

                    clipsSelected.Clear();
                    clipsSharedProperties.Clear();
                    clipsPathToSharedProperty.Clear();

                    clipsSelected = Selection.GetFiltered<AnimationClip>(SelectionMode.Assets).ToList();
                    
                    bool hasInstance = EditorWindow.HasOpenInstances<AREditor>();
                    if (hasInstance) EditorWindow.GetWindow(typeof(AREditor), false, null, false).Repaint();

                    foreach (AnimationClip clip in clipsSelected)
                    {
                        EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                        EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                        foreach (EditorCurveBinding binding in floatCurves) GetValues(binding);
                        foreach (EditorCurveBinding binding in objectCurves) GetValues(binding);

                        void GetValues(EditorCurveBinding binding)
                        {
                            if (clipsPathToSharedProperty.TryGetValue(binding.path, out ClipsSharedProperty sp))
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
                                clipsPathToSharedProperty.Add(binding.path, sp2);
                                sp2.oldPath = binding.path;
                                sp2.newPath = binding.path;
                                sp2.foldoutClips.Add(clip);
                                sp2.count++;

                                if (invalidPathToSharedProperty.TryGetValue(binding.path, out InvalidSharedProperty sp3))
                                {
                                    sp2.invalid = true;
                                }
                            }
                        }
                    }
                }
            }

            public static void RenameClipPaths(AnimationClip[] clips, bool replaceEntire, string oldPath, string newPath)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();

                    foreach (AnimationClip clip in clips)
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

                                    PlayerPrefs.SetInt("ARRepathCount", PlayerPrefs.GetInt("ARRepathCount") + 1);
                                }

                                if (replaceEntire && binding.path == oldPath)
                                {
                                    AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                                    binding.path = newPath;
                                    AnimationUtility.SetObjectReferenceCurve(clip, binding, objectCurve);

                                    PlayerPrefs.SetInt("ARRepathCount", PlayerPrefs.GetInt("ARRepathCount") + 1);
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

                                    PlayerPrefs.SetInt("ARRepathCount", PlayerPrefs.GetInt("ARRepathCount") + 1);
                                }

                                if (replaceEntire && binding.path == oldPath)
                                {
                                    AnimationUtility.SetEditorCurve(clip, binding, null);
                                    binding.path = newPath;
                                    AnimationUtility.SetEditorCurve(clip, binding, floatCurve);

                                    PlayerPrefs.SetInt("ARRepathCount", PlayerPrefs.GetInt("ARRepathCount") + 1);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                    GetClipPaths();
                    ARAutomatic.HierarchyChanged();
                }
            }
        }
    }
}
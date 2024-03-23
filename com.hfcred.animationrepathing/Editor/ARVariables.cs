using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if VRC_AVATARS
using VRC.SDK3.Avatars.Components;
#endif

namespace AnimationRepathing
{
#if VRC_AVATARS
    public static class PlayableExtension
    {
        public static VRCAvatarDescriptor.AnimLayerType ToAnimLayer(this ARVariables.Playables playable)
        {
            switch (playable)
            {
                case ARVariables.Playables.Base: return VRCAvatarDescriptor.AnimLayerType.Base;
                case ARVariables.Playables.Additive: return VRCAvatarDescriptor.AnimLayerType.Additive;
                case ARVariables.Playables.Gesture: return VRCAvatarDescriptor.AnimLayerType.Gesture;
                case ARVariables.Playables.Action: return VRCAvatarDescriptor.AnimLayerType.Action;
                case ARVariables.Playables.FX: return VRCAvatarDescriptor.AnimLayerType.FX;
                case ARVariables.Playables.Sitting: return VRCAvatarDescriptor.AnimLayerType.Sitting;
                case ARVariables.Playables.TPose: return VRCAvatarDescriptor.AnimLayerType.TPose;
                case ARVariables.Playables.IKPose: return VRCAvatarDescriptor.AnimLayerType.IKPose;
                case ARVariables.Playables.all: return VRCAvatarDescriptor.AnimLayerType.Deprecated0;
            }

            return VRCAvatarDescriptor.AnimLayerType.FX;
        }
    }
#endif

    public class ARVariables
    {
        static ARVariables()
        {
            Selection.selectionChanged -= getAutomaticController;
            Selection.selectionChanged += getAutomaticController;
        }

        public static void getAutomaticController()
        {
            if (getControllerAutomatically)
            {
                if (Selection.activeGameObject == null)
                {
                    automaticAnimator = null;
                    return;
                }
                if (!Selection.activeGameObject.GetComponentInParent<Animator>()?.runtimeAnimatorController)
                {
                    automaticAnimator = null;
                    return;
                }

                var compare = Selection.activeGameObject.GetComponentInParent<Animator>();

                if (compare != automaticAnimator)
                {
                    automaticAnimator = compare; ARAutomatic.GetAllChildren();
                }
            }
        }

        public static string currentVersion;
        public static string newestVersion;
        public static bool availableUpdate;
        public static bool fetchingVersion;
        public static bool fetchingFailed;
        public static bool automaticIsEnabled;
        public static int toolSelection;
        public static int manualToolSelection;
        public static int controllerSelection;
        public static bool getControllerAutomatically;
        public static int languageSelection;
        public static bool sendWarning;
        public static bool warnOnlyIfUsed;
        public static bool activeInBackground;
        public static bool disableDebugLogging;
        public static bool disableTooltips;
        public static GUIStyle fontStyle = new GUIStyle();

        public class InvalidSharedProperty
        {
            public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
            public bool foldout;
            public string oldPath;
            public string newPath;
            public int count;
        }
        public static List<InvalidSharedProperty> invalidSharedProperties = new List<InvalidSharedProperty>();
        public static Dictionary<string, InvalidSharedProperty> invalidPathToSharedProperty = new Dictionary<string, InvalidSharedProperty>();

        public class ClipsSharedProperty
        {
            public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
            public bool foldout;
            public string oldPath;
            public string newPath;
            public int count;
            public bool warning;
            public bool invalid;
        }
        public static List<ClipsSharedProperty> clipsSharedProperties = new List<ClipsSharedProperty>();
        public static Dictionary<string, ClipsSharedProperty> clipsPathToSharedProperty = new Dictionary<string, ClipsSharedProperty>();
        public static List<AnimationClip> clipsSelected = new List<AnimationClip>();
        public static string clipsReplaceFrom = string.Empty;
        public static string clipsReplaceTo = string.Empty;
        public static bool clipsReplaceFoldout;

        public static readonly List<ARAutomatic.HierarchyTransform> hierarchyTransforms = new List<ARAutomatic.HierarchyTransform>();
        public static readonly Dictionary<string, string> changedPaths = new Dictionary<string, string>();
        public static int hierarchyHash;
        public static Animator _animator;
        public static Animator _automaticAnimator;
        public static GameObject _avatar;
        public static Animator Animator
        {
            get
            {
                return EnsureNull(_animator);
            }
            set
            {
                if (_animator != value)
                {
                    _animator = value;
                    ARAutomatic.GetAllChildren();
                }
            }
        }
        public static Animator automaticAnimator
        {
            get
            {
                return EnsureNull(_automaticAnimator);
            }
            set
            {
                if (_automaticAnimator != value)
                {
                    _automaticAnimator = value;
                    ARAutomatic.GetAllChildren();
                }
            }
        }

        public static GameObject Avatar
        {
            get
            {
                return EnsureNull(_avatar);
            }
            set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    ARAutomatic.GetAllChildren();
                }
            }
        }

        public enum Playables
        {
            Base = 1 << 0,
            Additive = 1 << 1,
            Gesture = 1 << 2,
            Action = 1 << 3,
            FX = 1 << 4,
            Sitting = 1 << 5,
            TPose = 1 << 6,
            IKPose = 1 << 7,
            all = ~0
        }

        public static Playables PlayableSelection = Playables.all;

        /// <summary>
        /// Uses the currently selected target from the settings and
        /// returns either the Transform of the Animator Component
        /// OR the Transform of the Avatar. 
        /// </summary>
        public static Transform GetRoot()
        {
            if (getControllerAutomatically)
            {
                if (!Selection.activeGameObject) return null;
                if (!Selection.activeGameObject.GetComponentInParent<Animator>()?.runtimeAnimatorController) return null;

                return Selection.activeGameObject.GetComponentInParent<Animator>().transform;
            }

            return controllerSelection == 0 ?
            Animator?.transform : Avatar?.transform;
        }

        /// <summary>
        /// Uses the currently selected target from the settings and
        /// returns either the Animator Controller from the Animator Component
        /// OR the Animator Controllers from the selected layers on an Avatar. 
        /// </summary>
        public static List<AnimatorController> GetControllers()
        {
            List<AnimatorController> targetControllers = new List<AnimatorController>();

            if (getControllerAutomatically)
            {
                if (!Selection.activeGameObject) return targetControllers;
                if (!Selection.activeGameObject.GetComponentInParent<Animator>()?.runtimeAnimatorController) return targetControllers;

                targetControllers.Add(Selection.activeGameObject.GetComponentInParent<Animator>().runtimeAnimatorController as AnimatorController);
            }
            else if (controllerSelection == 0 && Animator != null && Animator.runtimeAnimatorController != null)
            {
                targetControllers.Add(Animator.runtimeAnimatorController as AnimatorController);
            }
#if VRC_AVATARS
            else if (controllerSelection == 1 && Avatar != null && Avatar.GetComponent<VRCAvatarDescriptor>() != null)
            {
                VRCAvatarDescriptor descriptor = Avatar.GetComponent<VRCAvatarDescriptor>();

                foreach (Playables playable in Enum.GetValues(typeof(Playables)))
                {
                    if ((playable & PlayableSelection) != 0)
                    {
                        if (descriptor.baseAnimationLayers.Count() == 5 && descriptor.baseAnimationLayers[3].type == VRCAvatarDescriptor.AnimLayerType.FX)
                        {
                            descriptor.baseAnimationLayers[3].type = VRCAvatarDescriptor.AnimLayerType.Action;
                        }

                        VRCAvatarDescriptor.AnimLayerType VRCType = playable.ToAnimLayer();
                        foreach (var customAnimLayer in descriptor.baseAnimationLayers.Concat(descriptor.specialAnimationLayers).Where(x => x.type == VRCType))
                        {
                            if (EnsureNull(customAnimLayer.animatorController) != null)
                            {
                                targetControllers.Add(customAnimLayer.animatorController as AnimatorController);
                            }
                        }
                    }
                }
            }
#endif

            return targetControllers;
        }

        private static T EnsureNull<T>(T obj) where T : UnityEngine.Object
        {
            if (obj == null) obj = null;
            return obj;
        }
    }
}

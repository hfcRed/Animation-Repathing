using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using System;
using VRC.SDK3.Avatars.Components;

namespace AutoAnimationRepath
{
    public class AARVariables
    {
        public static bool automaticIsEnabled;
        public static bool renameActive = true;
        public static bool reparentActive = true;
        public static bool renameWarning = true;
        public static bool warnOnlyIfUsed = false;
        public static bool reparentWarning = true;
        public static bool activeInBackground = false;
        public static bool disableTooltips = false;

        public static int toolSelection;
        public static int manualToolSelection;
        public static int controllerSelection;
        public static int languageSelection;

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
        public static int invalidPosition;

        public class ClipsSharedProperty
        {
            public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
            public bool foldout;
            public string oldPath;
            public string newPath;
            public int count;
            public bool warning;
        }
        public static List<ClipsSharedProperty> clipsSharedProperties = new List<ClipsSharedProperty>();
        public static Dictionary<string, ClipsSharedProperty> clipsPathToSharedProperty = new Dictionary<string, ClipsSharedProperty>();
        public static List<AnimationClip> clipsSelected = new List<AnimationClip>();
        public static string clipsReplaceFrom = string.Empty;
        public static string clipsReplaceTo = string.Empty;
        public static bool clipsReplaceFoldout;

        public static readonly List<AARAutomatic.HierarchyTransform> hierarchyTransforms = new List<AARAutomatic.HierarchyTransform>();
        public static readonly Dictionary<string, string> changedPaths = new Dictionary<string, string>();
        public static Animator _animator;
        public static GameObject _avatar;
        public static Animator animator
        {
            get => _animator;
            set
            {
                if (_animator != value)
                {
                    _animator = value;
                    AARAutomatic.OnRootChanged();
                }
            }
        }
        public static GameObject avatar
        {
            get => _avatar;
            set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    AARAutomatic.OnRootChanged();
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

        public static Transform GetRoot()
        {
            return controllerSelection == 0 ?
            animator == null ? null : animator.transform : avatar == null ? null : avatar.transform;
        }

        private static T EnsureNull<T>(T obj) where T : UnityEngine.Object
        {
            if (obj == null) obj = null;
            return obj;
        }

        public static List<AnimatorController> GetControllers()
        {
            List<AnimatorController> targetControllers = new List<AnimatorController>();

            if (controllerSelection == 0 && animator != null)
            {
                targetControllers.Add(animator?.runtimeAnimatorController as AnimatorController);
            }
            else if (controllerSelection == 1 && avatar != null && avatar.GetComponent<VRCAvatarDescriptor>() != null)
            {
#if VRC_SDK_VRCSDK3
                VRCAvatarDescriptor descriptor = avatar.GetComponent<VRCAvatarDescriptor>();

                foreach (Playables playable in Enum.GetValues(typeof(Playables)))
                {
                    if ((playable & PlayableSelection) != 0)
                    {
                        switch (playable)
                        {
                            case Playables.Base:
                                if (EnsureNull(descriptor.baseAnimationLayers[0].animatorController) != null)
                                    targetControllers.Add(descriptor.baseAnimationLayers[0].animatorController as AnimatorController);
                                break;
                            case Playables.Additive:
                                if (EnsureNull(descriptor.baseAnimationLayers[1].animatorController) != null)
                                    targetControllers.Add(descriptor.baseAnimationLayers[1].animatorController as AnimatorController);
                                break;
                            case Playables.Gesture:
                                if (EnsureNull(descriptor.baseAnimationLayers[2].animatorController) != null)
                                    targetControllers.Add(descriptor.baseAnimationLayers[2].animatorController as AnimatorController);
                                break;
                            case Playables.Action:
                                if (EnsureNull(descriptor.baseAnimationLayers[3].animatorController) != null)
                                    targetControllers.Add(descriptor.baseAnimationLayers[3].animatorController as AnimatorController);
                                break;
                            case Playables.FX:
                                if (EnsureNull(descriptor.baseAnimationLayers[4].animatorController) != null)
                                    targetControllers.Add(descriptor.baseAnimationLayers[4].animatorController as AnimatorController);
                                break;
                            case Playables.Sitting:
                                if (EnsureNull(descriptor.specialAnimationLayers[0].animatorController) != null)
                                    targetControllers.Add(descriptor.specialAnimationLayers[0].animatorController as AnimatorController);
                                break;
                            case Playables.TPose:
                                if (EnsureNull(descriptor.specialAnimationLayers[1].animatorController) != null)
                                    targetControllers.Add(descriptor.specialAnimationLayers[1].animatorController as AnimatorController);
                                break;
                            case Playables.IKPose:
                                if (EnsureNull(descriptor.specialAnimationLayers[2].animatorController) != null)
                                    targetControllers.Add(descriptor.specialAnimationLayers[2].animatorController as AnimatorController);
                                break;
                        }
                    }
                }
#endif
            }
            return targetControllers;
        }
    }
}

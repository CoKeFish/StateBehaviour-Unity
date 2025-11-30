using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Represents a utility class designed to encapsulate and manage time-related configurations.
    ///     Commonly used for defining parameters such as delays, durations, or intervals in various operations.
    /// </summary>
    [Serializable]
    [HideLabel]
    [HideReferenceObjectPicker]
    public class TimeWrapper
    {
        #region Serialized Fields

        /// <summary>
        ///     Determines whether a custom delay should be applied before starting the tween or animation.
        ///     When set to true, the delay configured in the associated property will be used,
        ///     allowing for precise control over the timing behavior.
        /// </summary>
        [Title("Timing Settings")]
        [LabelWidth(140)]
        [Tooltip("If true, a custom delay will be applied before starting the tween.")]
        [SerializeField]
        public bool useCustomDelay;

        /// <summary>
        ///     Specifies a custom delay, in seconds, to be applied before starting a tween or animation,
        ///     when <c>useCustomDelay</c> is set to <c>true</c>.
        /// </summary>
        /// <remarks>
        ///     The value must be non-negative and is only applicable if the <c>useCustomDelay</c> flag is enabled.
        /// </remarks>
        [ShowIf(nameof(useCustomDelay))] [LabelWidth(140)] [Min(0)] [SerializeField]
        public float customDelay;


        /// <summary>
        ///     Indicates whether a custom duration value should be used for animations or behaviors
        ///     instead of the default duration.
        /// </summary>
        [LabelWidth(140)] [Tooltip("If true, a custom duration will be used.")] [SerializeField]
        public bool useCustomDuration;

        /// <summary>
        ///     Represents the length of time for an activity, event, or process.
        /// </summary>
        /// <remarks>
        ///     This variable is intended to store a time interval, typically expressed in units such as seconds, minutes, or
        ///     hours,
        ///     depending on the context in which it is used. The specific format or unit of measurement should be clearly defined
        ///     elsewhere in the program.
        /// </remarks>
        [ShowIf(nameof(useCustomDuration))] [LabelWidth(140)] [Min(0)] [SerializeField]
        public float duration = 0.5f;

        /// <summary>
        ///     Indicates whether a custom easing function should be applied to the animation or behavior.
        ///     When set to true, the `ease` parameter can be specified to customize how the animation interpolates over time.
        /// </summary>
        [LabelWidth(140)] [Tooltip("If true, a custom ease curve will be used.")] [SerializeField]
        public bool useCustomEase;

        /// <summary>
        ///     Represents the rate of change in a value over time, commonly used in animations
        ///     or transitions to define how the intermediate values are calculated between
        ///     a starting and an ending point.
        /// </summary>
        [ShowIf(nameof(useCustomEase))] [LabelWidth(140)] [SerializeField]
        public Ease ease = Ease.OutQuad;

        #endregion
    }
}
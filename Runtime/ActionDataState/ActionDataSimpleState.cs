#if STATE_BEHAVIOR_ENABLED
using System;
using Ardalis.GuardClauses;
using DG.Tweening;
using LanguageExt;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Simple state configuration that controls timing, easing and value blending for tweened actions.
    /// </summary>
    [Serializable]
    public class ActionDataSimpleState<TValue> : ActionDataState<TValue>
    {
        #region Serialized Fields

        /// <summary>
        ///     Represents a customizable timing configuration used to control delay,
        ///     duration, and easing behavior for tweened state actions.
        /// </summary>
        public TimeWrapper time = new();

        /// <summary>
        ///     Determines whether the original value should be used instead of the configured target.
        /// </summary>
        [Title("Value Settings")]
        [LabelWidth(140)]
        [Tooltip("If true, uses the original value instead of setting a target.")]
        [DisableIf(nameof(_isDefaultState))]
        public bool useOrigin;

        /// <summary>
        ///     Target value applied when the tween completes.
        /// </summary>
        [ShowIf(nameof(ShowEndValue))]
        [LabelWidth(140)]
        [Tooltip("The target value reached at the end of the animation.")]
        public TValue endValue;

        #endregion

        #region Fields

        /// <summary>
        ///     Indicates whether this configuration belongs to the default enum state.
        /// </summary>
        private readonly bool _isDefaultState;

        #endregion

        #region Properties

        /// <summary>
        ///     Determines whether the end value field should be drawn in the inspector.
        /// </summary>
        private bool ShowEndValue => !_isDefaultState && !useOrigin;

        #endregion

        #region Constructors and Injected

        /// <summary>
        ///     Creates a new simple state configuration.
        /// </summary>
        /// <param name="isDefaultState">Indicates whether the configuration is for the enum default value.</param>
        public ActionDataSimpleState(bool isDefaultState = false)
        {
            useOrigin = isDefaultState;
            _isDefaultState = isDefaultState;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Applies the configuration to the supplied tweener while considering the original and target values.
        /// </summary>
        /// <param name="tweener">The tween instance to configure.</param>
        /// <param name="originalValue">The original value captured from the source component.</param>
        /// <param name="externalEndValue">The optional external target value to apply to the configuration.</param>
        /// <param name="timeWrapper">The wrapper object used to manage time-related configurations.</param>
        /// <returns>The configured tween instance.</returns>
        public override Tweener ApplyData(Tweener tweener,
            TValue originalValue,
            Option<TValue> externalEndValue,
            TimeWrapper timeWrapper)
        {
            Guard.Against.Null(tweener,
                "Tweener es nulo. No se puede aplicar ActionDataSimpleContainer.");

            var delay = time.useCustomDelay ? time.customDelay : timeWrapper.customDelay;
            var duration = time.useCustomDuration ? time.customDelay : timeWrapper.duration;
            var ease = time.useCustomEase ? time.ease : timeWrapper.ease;

            var endValueToUse = useOrigin
                ? originalValue
                : externalEndValue.Match(
                    v => v,
                    () => endValue
                );

            // Aplica easing, delay y duración
            tweener
                .SetEase(ease)
                .SetDelay(delay)
                .ChangeEndValue(endValueToUse, duration, true)
                .Restart(true, delay);

            return tweener;
        }

        /// <summary>
        ///     Applies a configuration to the specified tweener instance for instant changes.
        /// </summary>
        /// <param name="tweener">The tween instance to which the configuration will be applied.</param>
        /// <param name="originalValue">The original value of the property being tweened.</param>
        /// <param name="externalEndValue">An optional external value to override the default target end value.</param>
        /// <returns>The configured tweener instance.</returns>
        public override Tweener ApplyDataInstant(Tweener tweener,
            TValue originalValue,
            Option<TValue> externalEndValue)
        {
            Guard.Against.Null(tweener,
                "Tweener es nulo. No se puede aplicar ActionDataSimpleContainer.");

            const float delay = 0f;
            const float duration = 0f;

            var endValueToUse = useOrigin
                ? originalValue
                : externalEndValue.Match(
                    v => v,
                    () => endValue
                );

            // Aplica easing, delay y duración
            tweener
                .SetDelay(delay)
                .ChangeEndValue(endValueToUse, duration, true)
                .Restart();

            return tweener;
        }

        #endregion
    }
}
#endif
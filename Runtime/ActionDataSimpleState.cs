#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
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
        ///     Indicates whether a custom delay should be applied before the tween starts.
        /// </summary>
        [Title("Timing Settings")]
        [LabelWidth(140)]
        [Tooltip("If true, a custom delay will be applied before showing.")]
        public bool useCustomDelay;

        /// <summary>
        ///     Specifies the custom delay to use when <see cref="useCustomDelay" /> is enabled.
        /// </summary>
        [ShowIf(nameof(useCustomDelay))] [LabelWidth(140)]
        public float customDelay;

        /// <summary>
        ///     Duration of the tween in seconds.
        /// </summary>
        [LabelWidth(140)] public float duration = 0.5f;

        /// <summary>
        ///     Easing function applied to the tween over time.
        /// </summary>
        [LabelWidth(140)] [Tooltip("Ease curve used by the animation.")]
        public Ease easeShow = Ease.OutQuad;

        /// <summary>
        ///     Determines whether the original value should be used instead of the configured target.
        /// </summary>
        [ShowIf(nameof(_isDefaultState))]
        [Title("Value Settings")]
        [LabelWidth(140)]
        [Tooltip("If true, uses the original value instead of setting a target.")]
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
        private bool _isDefaultState;

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
        ///     Applies the configuration to the supplied <see cref="Tweener" /> while considering the original value.
        /// </summary>
        /// <param name="tweener">Tween instance to configure.</param>
        /// <param name="originalValue">Value captured from the source component.</param>
        /// <returns>The configured tween instance.</returns>
        public override Tweener ApplyData(Tweener tweener, TValue originalValue)
        {
            if (tweener == null)
            {
                Debug.LogWarning("Tweener es nulo. No se puede aplicar ActionDataSimpleContainer.");
                return null;
            }

            var delay = useCustomDelay ? customDelay : tweener.Delay();

            // Aplica easing, delay y duración
            tweener
                .SetEase(easeShow)
                .SetDelay(delay)
                .ChangeEndValue(useOrigin ? originalValue : endValue, duration, true);

            return tweener;
        }

        /// <summary>
        ///     Applies the configuration to the supplied <see cref="Tweener" /> without considering an original value.
        /// </summary>
        /// <param name="tweener">Tween instance to configure.</param>
        /// <returns>The configured tween instance.</returns>
        public override Tweener ApplyData(Tweener tweener)
        {
            if (tweener == null)
            {
                Debug.LogWarning("Tweener es nulo. No se puede aplicar ActionDataSimpleContainer.");
                return null;
            }

            var delay = useCustomDelay ? customDelay : tweener.Delay();

            // Aplica easing, delay y duración
            tweener
                .SetEase(easeShow)
                .SetDelay(delay)
                .ChangeEndValue(endValue, duration, true);

            return tweener;
        }

        #endregion
    }
}
#endif
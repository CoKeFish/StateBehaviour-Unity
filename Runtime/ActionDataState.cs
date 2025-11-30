#if STATE_BEHAVIOR_ENABLED
using DG.Tweening;
using LanguageExt;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Represents a runtime configuration that can be applied to a tween for a specific state.
    /// </summary>
    public abstract class ActionDataState<TValue>
    {
        #region Methods

        /// <summary>
        ///     Applies the configuration to the provided tween using an original value as reference, an optional end value, and
        ///     timing settings.
        /// </summary>
        /// <param name="tweener">Tween instance to configure.</param>
        /// <param name="originalValue">The initial value used as a reference.</param>
        /// <param name="externalEndValue">An optional final value to be applied to the tween.</param>
        /// <param name="timeWrapper">The timing settings to configure delay and duration of the tween.</param>
        /// <returns>The configured tween instance.</returns>
        public abstract Tweener ApplyData(
            Tweener tweener,
            TValue originalValue,
            Option<TValue> externalEndValue,
            TimeWrapper timeWrapper);

        /// <summary>
        ///     Applies the configuration to the provided tween with the given original value and an optional end value,
        ///     immediately updating the tween state without any delay or duration.
        /// </summary>
        /// <param name="tweener">The tween instance to configure and update instantly.</param>
        /// <param name="originalValue">The initial value used as a reference for the operation.</param>
        /// <param name="externalEndValue">An optional final value to override the default end value of the tween.</param>
        /// <returns>The updated tween instance after applying the configuration instantly.</returns>
        public abstract Tweener ApplyDataInstant(
            Tweener tweener,
            TValue originalValue,
            Option<TValue> externalEndValue);

        #endregion
    }
}
#endif
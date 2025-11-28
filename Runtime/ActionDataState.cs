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
        ///     Applies the configuration to the provided tween using an original value as reference and an end value.
        /// </summary>
        /// <param name="tweener">Tween instance to configure.</param>
        /// <param name="originalValue">The initial value used as a reference.</param>
        /// <param name="externalEndValue">The final value to be applied to the tween.</param>
        /// <returns>The configured tween instance.</returns>
        public abstract Tweener ApplyData(Tweener tweener,
            TValue originalValue,
            Option<TValue> externalEndValue);

        /// <summary>
        ///     Applies the configuration to the provided tween without an original value.
        /// </summary>
        /// <param name="tweener">Tween instance to configure.</param>
        /// <param name="externalEndValue">The final value to be applied to the tween.</param>
        /// <returns>The configured tween instance.</returns>
        public abstract Tweener ApplyData(Tweener tweener, Option<TValue> externalEndValue);

        #endregion
    }
}
#endif
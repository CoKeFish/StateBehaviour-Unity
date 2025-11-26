#if STATE_BEHAVIOR_ENABLED
using DG.Tweening;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Represents a runtime configuration that can be applied to a tween for a specific state.
    /// </summary>
    public abstract class ActionDataState<TValue>
    {
        #region Methods

        /// <summary>
        ///     Applies the configuration to the provided tween using an original value as reference.
        /// </summary>
        /// <param name="tweener">Tween instance to configure.</param>
        /// <param name="originalValue">Captured original value.</param>
        /// <returns>The configured tween instance.</returns>
        public abstract Tweener ApplyData(Tweener tweener,
            TValue originalValue);

        /// <summary>
        ///     Applies the configuration to the provided tween without an original value.
        /// </summary>
        /// <param name="tweener">Tween instance to configure.</param>
        /// <returns>The configured tween instance.</returns>
        public abstract Tweener ApplyData(Tweener tweener);

        #endregion
    }
}
#endif
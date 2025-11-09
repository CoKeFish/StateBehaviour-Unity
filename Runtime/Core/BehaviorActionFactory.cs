#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Defines the available behaviors that can be applied to a tween for a given state.
    /// </summary>
    [Serializable]
    public enum BehaviorActionTypes
    {
        /// <summary>
        ///     No behavior will be applied.
        /// </summary>
        None,
        /// <summary>
        ///     A basic tween configuration with a single playthrough.
        /// </summary>
        Simple,
        /// <summary>
        ///     Executes the tween and optionally loops.
        /// </summary>
        OneShot,
        /// <summary>
        ///     Plays the tween in a continuous looping manner.
        /// </summary>
        Looping,
        /// <summary>
        ///     Executes a sequenced set of tweens.
        /// </summary>
        Sequencer
    }

    /// <summary>
    ///     Applies shared configuration to tweeners depending on the requested behavior type.
    /// </summary>
    public static class BehaviorActionFactory
    {
        #region Methods

        /// <summary>
        ///     Configures a tweener instance based on the desired behavior.
        /// </summary>
        /// <param name="type">Behavior type to apply.</param>
        /// <param name="tweener">Tween instance to configure.</param>
        public static void Set(BehaviorActionTypes type, Tweener tweener)
        {
            switch (type)
            {
                case BehaviorActionTypes.None:
                    break;
                case BehaviorActionTypes.Simple:
                    tweener.SetAutoKill(false)
                        .SetLoops(1, LoopType.Yoyo);
                    return;
                case BehaviorActionTypes.Looping:
                    break;
                case BehaviorActionTypes.Sequencer:
                    break;
                case BehaviorActionTypes.OneShot:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        #endregion
    }
}
#endif
#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    /// Represents a behavior type that executes a tween instantly,
    /// bypassing any animation or delay.
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
        /// Represents a behavior type that performs an action instantaneously
        /// without any animation, transition, or delay.
        /// </summary>
        Instant,

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
            if (tweener == null) return;

            switch (type)
            {
                case BehaviorActionTypes.None:
                    // No configuration needed
                    break;
                    
                case BehaviorActionTypes.Simple:
                    tweener.SetAutoKill(false)
                        .SetLoops(1, LoopType.Yoyo);
                    break;
                    
                case BehaviorActionTypes.Instant:
                    // Configure tween to execute instantly (duration 0)
                    tweener.SetAutoKill(false)
                        .Complete(true);
                    break;
                    
                case BehaviorActionTypes.Looping:
                    // Configure tween for infinite looping
                    tweener.SetAutoKill(false)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                    
                case BehaviorActionTypes.Sequencer:
                    // Sequencer behavior will be implemented separately
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        #endregion
    }
}
#endif
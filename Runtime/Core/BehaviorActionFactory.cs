#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;

namespace Marmary.StateBehavior.Core
{
    [Serializable]
    public enum BehaviorActionTypes
    {
        None,
        Simple,
        OneShot,
        Looping,
        Sequencer
    }

    public static class BehaviorActionFactory
    {
        #region Methods

        public static void Set(BehaviorActionTypes type, Tweener Tweener)
        {
            switch (type)
            {
                case BehaviorActionTypes.None:
                    break;
                case BehaviorActionTypes.Simple:
                    Tweener.SetAutoKill(false)
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
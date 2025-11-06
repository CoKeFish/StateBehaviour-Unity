#if STATE_BEHAVIOR_ENABLED
using DG.Tweening;

namespace Marmary.StateBehavior.Core
{
    public abstract class ActionDataState<TValue>
    {
        #region Methods

        public abstract Tweener ApplyData(Tweener tweener,
            TValue originalValue);

        public abstract Tweener ApplyData(Tweener tweener);

        #endregion
    }
}
#endif
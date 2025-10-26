using DG.Tweening;

namespace Scrips.StateBehavior
{
    public abstract class ActionDataState<TValue>
    {
        public abstract Tweener ApplyData(Tweener tweener,
            TValue originalValue);
        
        public abstract Tweener ApplyData(Tweener tweener);

    }
}
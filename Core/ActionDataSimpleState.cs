using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scrips.StateBehavior
{
    [Serializable]
    public class ActionDataSimpleState<TValue> : ActionDataState<TValue>
    {
        [Title("Timing Settings")]
        [LabelWidth(140)]
        [Tooltip("If true, a custom delay will be applied before showing.")]
        public bool useCustomDelay;

        [ShowIf(nameof(useCustomDelay))]
        [LabelWidth(140)]
        public float customDelay = 0f;
        
        [LabelWidth(140)]
        public float duration = 0.5f;

        [LabelWidth(140)]
        [Tooltip("Ease curve used by the animation.")]
        public Ease easeShow = Ease.OutQuad;

        [ShowIf(nameof(IsDefaultState))]
        [Title("Value Settings")]
        [LabelWidth(140)]
        [Tooltip("If true, uses the original value instead of setting a target.")]
        public bool useOrigin = false;

        [ShowIf(nameof(ShowEndValue))]
        [LabelWidth(140)]
        [Tooltip("The target value reached at the end of the animation.")]
        public TValue endValue ;

        private bool IsDefaultState = false;
        private bool ShowEndValue => !useOrigin;

        private readonly bool showOrigin;


        public ActionDataSimpleState(bool isDefaultState = false)
        {
            useOrigin = isDefaultState;
            IsDefaultState = isDefaultState;
            showOrigin = !isDefaultState;
        }
        
        
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

    }
}
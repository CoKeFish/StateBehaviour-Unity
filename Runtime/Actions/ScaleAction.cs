#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.Actions
{
    /// <summary>
    ///     Tweens the local scale of a <see cref="RectTransform" /> in response to selectable state changes.
    /// </summary>
    public abstract class ScaleAction<TState, TActionData> : Action<TState, Vector3, RectTransform, TActionData>
        where TState : Enum where TActionData : ActionData<TState, Vector3>
    {
        #region Methods

        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            return DOTween.To(
                () => target.transform.localScale,
                x => target.transform.localScale = x,
                originalValue,
                0f // duración temporal
            ).Pause();
        }

        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = gameObject.transform.localScale;
        }

        #endregion
    }
}
#endif
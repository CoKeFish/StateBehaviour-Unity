#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.Actions
{
    /// <summary>
    ///     Animates the text color of a <see cref="TextMeshProUGUI" /> when selectable states change.
    /// </summary>
    public abstract class ColorFontAction<TState, TActionData> : Action<TState, Color, TextMeshProUGUI, TActionData>
        where TState : Enum where TActionData : ActionData<TState, Color>
    {
        #region Methods

        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            return DOTween.To(
                () => target.color,
                x => target.color = x,
                originalValue,
                0f // duración temporal
            ).Pause();
        }

        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = target.color;
        }

        #endregion
    }
}
#endif
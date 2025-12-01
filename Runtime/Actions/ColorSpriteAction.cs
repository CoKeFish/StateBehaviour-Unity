#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Marmary.StateBehavior.Runtime.Actions
{
    /// <summary>
    ///     Animates the color of a <see cref="UnityEngine.UI.Image" /> component when selectable states change.
    /// </summary>
    public abstract class ColorSpriteAction<TState, TActionData> : Action<TState, Color, Image, TActionData>
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
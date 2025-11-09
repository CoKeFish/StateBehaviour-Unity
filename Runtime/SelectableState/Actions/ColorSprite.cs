#if STATE_BEHAVIOR_ENABLED
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    /// <summary>
    ///     Animates the color of a <see cref="UnityEngine.UI.Image" /> component when selectable states change.
    /// </summary>
    public class ColorSpriteSelectableAction : SelectableAction<Image, Color>
    {
        #region Methods

        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            return DOTween.To(
                () => target.color,
                x => target.color = x,
                OriginalValue,
                0f // duración temporal
            ).Pause();
        }

        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            OriginalValue = target.color;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<ColorSpriteSelectableActionData>();
        }
#endif

        #endregion
    }
}
#endif
#if STATE_BEHAVIOR_ENABLED
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Animates the text color of a <see cref="TextMeshProUGUI" /> when selectable states change.
    /// </summary>
    public class ColorFontSelectableAction : SelectableAction<TextMeshProUGUI, Color>
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

#if UNITY_EDITOR
        /// <inheritdoc />
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<ColorFontSelectableActionData>();
        }
#endif

        #endregion
    }
}
#endif
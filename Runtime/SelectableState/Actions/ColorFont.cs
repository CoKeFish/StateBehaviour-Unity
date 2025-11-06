#if STATE_BEHAVIOR_ENABLED
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    public class ColorFontSelectableAction : SelectableAction<TextMeshProUGUI, Color>
    {
        #region Methods

        protected override Tweener CreateTweener(GameObject gameObject)
        {
            return DOTween.To(
                () => target.color,
                x => target.color = x,
                originalValue,
                0f // duración temporal
            ).Pause();
        }

        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = target.color;
        }

#if UNITY_EDITOR
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<ColorFontSelectableActionData>();
        }
#endif

        #endregion
    }
}
#endif
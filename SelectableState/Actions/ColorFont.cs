using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Scrips.StateBehavior
{
    public class ColorFontSelectableAction : SelectableAction<TextMeshProUGUI, Color>
    {
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
    }
}
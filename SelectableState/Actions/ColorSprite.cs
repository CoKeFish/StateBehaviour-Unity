using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    public class ColorSpriteSelectableAction : SelectableAction<Image, Color>
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
            return ScriptableObject.CreateInstance<ColorSpriteSelectableActionData>();
        }
#endif
    }
}
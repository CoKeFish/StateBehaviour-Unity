using DG.Tweening;
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    public class ScaleSelectableAction : SelectableAction<RectTransform, Vector3>
    {
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            return DOTween.To(
                () => target.transform.localScale,
                x => target.transform.localScale = x,
                originalValue,
                0f // duración temporal
            ).Pause();
        }

        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = gameObject.transform.localScale;
        }

#if UNITY_EDITOR
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<ScaleSelectableActionData>();
        }
#endif
    }
}
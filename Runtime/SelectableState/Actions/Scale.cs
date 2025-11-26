#if STATE_BEHAVIOR_ENABLED
using DG.Tweening;
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    /// <summary>
    ///     Tweens the local scale of a <see cref="RectTransform" /> in response to selectable state changes.
    /// </summary>
    public class ScaleSelectableAction : SelectableAction<RectTransform, Vector3>
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

#if UNITY_EDITOR
        /// <inheritdoc />
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<ScaleSelectableActionData>();
        }
#endif

        #endregion
    }
}
#endif
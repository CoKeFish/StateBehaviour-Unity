#if STATE_BEHAVIOR_ENABLED && DEFINICION_TexAnimator
// TODO: Revisar este archivo cuando se implemente la característica
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scrips.StateBehavior
{
    /// <summary>
    ///     Animates TextAnimator components by tweaking tagged behaviour parameters for each state.
    /// </summary>
    [Serializable]
    public class AnimationFontSelectableAction : SelectableAction<TextAnimator_TMP, float>
    {
        /// <summary>
        ///     The tag to use for the animation, see Febucci documentation for more info
        /// </summary>
        [SerializeField] [Required] private string tag = "Wave";

        /// <summary>
        ///     Stores the cached amount applied to the animation tag.
        /// </summary>
        private float _amount;

        /// <summary>
        ///     Gets or sets the animated amount and updates the underlying tag.
        /// </summary>
        private float Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                target.DefaultBehaviorsTags = new[] { $"{tag} a={_amount}".Replace(',', '.') };
            }
        }

        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            if (useCustomTarget)
            {
                return DOTween.To(
                    () => Amount,
                    x => Amount = x,
                    originalValue,
                    0f // duración temporal
                ).Pause();
            }
            else
            {
                return DOTween.To(
                    () => Amount,
                    x => Amount = x,
                    originalValue,
                    0f // duración temporal
                ).Pause();
            }
        }

        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = Amount;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<AnimationFontSelectableActionData>();
        }
#endif
    }


    /// <summary>
    ///     Scriptable object holding animation amplitudes for text animator actions.
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationFontSelectableActionData",
        menuName = "StateBehavior/Actions/AnimationFontSelectableActionData",
        order = 1)]
    public class AnimationFontSelectableActionData : ActionDataSelectable<float>
    {
    }
}
#endif
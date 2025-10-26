
#if DEFINICION_TexAnimator

using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scrips.StateBehavior
{
    [Serializable]
    public class AnimationFontSelectableAction : SelectableAction<TextAnimator_TMP,float>
    {
        
        /// <summary>
        ///     The tag to use for the animation, see Febucci documentation for more info
        /// </summary>
        [SerializeField] [Required]
        private string tag = "Wave";
        
        private float _amount;

        private float Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                target.DefaultBehaviorsTags = new[] { $"{tag} a={_amount}".Replace(',', '.') };
            }
        }
        
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

        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = Amount;
        }

#if UNITY_EDITOR
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<AnimationFontSelectableActionData>();
        }
#endif
    }


    [CreateAssetMenu(fileName = "AnimationFontSelectableActionData",
        menuName = "StateBehavior/Actions/AnimationFontSelectableActionData",
        order = 1)]
    public class AnimationFontSelectableActionData : ActionDataSelectable<float>
    {
        
    }
}

#endif
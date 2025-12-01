#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
using Febucci.UI;
using Marmary.StateBehavior.Runtime.SelectableState;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.Actions
{
    /// <summary>
    ///     Animates TextAnimator components by tweaking tagged behaviour parameters for each state.
    /// </summary>
    [Serializable]
    public class AnimationFontAction<TState, TActionData> : Action<TState, float, TextAnimator_TMP, TActionData>
        where TState : Enum where TActionData : ActionData<TState, float>
    {
        #region Serialized Fields

        /// <summary>
        ///     The tag to use for the animation, see Febucci documentation for more info
        /// </summary>
        [SerializeField] [Required] private string tag = "Wave";

        #endregion

        #region Fields

        /// <summary>
        ///     Stores the cached amount applied to the animation tag.
        /// </summary>
        private float _amount;

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            return DOTween.To(
                () => Amount,
                x => Amount = x,
                originalValue,
                0f // duración temporal
            ).Pause();
        }

        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = Amount;
        }

        #endregion
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
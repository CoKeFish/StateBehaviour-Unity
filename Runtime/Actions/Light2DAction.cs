using System;
using DG.Tweening;
using LanguageExt;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Marmary.StateBehavior.Runtime.Actions
{
    /// <summary>
    /// Represents an abstract class derived from the generic <see cref="Action{TState, TValue, TComponent, TActionData}"/>,
    /// specifically designed to modify the intensity of a <see cref="Light2D"/> component through state-based behavior and tweening.
    /// </summary>
    /// <typeparam name="TState">The type representing the state, constrained to an <see cref="Enum"/>.</typeparam>
    /// <typeparam name="TActionData">The data type associated with the action, constrained to <see cref="ActionData{TState, TValue}"/>.</typeparam>
    /// <remarks>
    /// This class is a specialization for managing animations of light intensity in 2D environments
    /// using the <c>DOTween</c> library and the <see cref="Light2D"/> component from Unity's Universal Render Pipeline.
    /// </remarks>
    public abstract class Light2DAction<TState, TActionData> : Action<TState, float, Light2D, TActionData>
        where TState : Enum where TActionData : ActionData<TState, float>
    {
        
        /// <summary>
        ///     RectTransform of the selectable
        /// </summary>
        private RectTransform _rectTransform;
        
        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            if (_rectTransform != null)
                return DOTween.To(
                    () => target.intensity,
                    x => target.intensity = x,
                    originalValue,
                    0f // duración temporal
                ).Pause();
            Debug.LogError("No RectTransform found on GameObject");
            return null;


        }
        
        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            originalValue = 0;
        }
        
        /// <summary>
        ///     Retrieves the end position value used during the transition.
        /// </summary>
        /// <returns>An optional value of type <see cref="Vector3" />, representing the target position for the transition.</returns>
        protected override Option<float> GetEndValue()
        {
            var rect = _rectTransform.rect;
            var with = rect.width / 2;
            var height = rect.height / 2;

            target.shapePath[0].x = with;
            target.shapePath[0].y = height;
            target.shapePath[1].x = -with;
            target.shapePath[1].y = height;
            target.shapePath[2].x = -with;
            target.shapePath[2].y = -height;
            target.shapePath[3].x = with;
            target.shapePath[3].y = -height;
            return base.GetEndValue();
        }
        
    }
}
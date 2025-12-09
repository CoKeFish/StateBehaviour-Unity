#if STATE_BEHAVIOR_ENABLED

using System;
using DG.Tweening;
using LanguageExt;
using Marmary.Utils.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.Actions
{
    /// <summary>
    ///     Switch action that animates RectTransform movement for show and hide transitions.
    ///     Moves elements in and out of screen based on configured hiding position.
    /// </summary>
    [Serializable]
    public class
        RectTransformMovementAction<TState, TActionData> : Action<TState, Vector3, RectTransform, TActionData>
        where TState : Enum where TActionData : ActionData<TState, Vector3>
    {
        #region Serialized Fields

        /// <summary>
        ///     Position where the element will be hidden (Top, Bottom, Left, Right).
        /// </summary>
        [Title("Movement Settings")]
        [LabelWidth(140)]
        [Tooltip("Position where the element will be hidden when transitioning to Hide state.")]
        public Position hidingPosition = Position.Top;

        /// <summary>
        ///     Scale correction factor for width calculations when hiding to left/right.
        /// </summary>
        [LabelWidth(140)] [Tooltip("Scale correction factor applied to width calculations for left/right hiding.")]
        public float scaleCorrection = 1f;

        #endregion

        #region Methods

        /// <summary>
        ///     Retrieves the end position value used during the transition.
        /// </summary>
        /// <returns>An optional value of type <see cref="Vector3" />, representing the target position for the transition.</returns>
        protected override Option<Vector3> GetEndValue()
        {
            return GetEndPosition();
        }

        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            if (target == null)
                target = gameObject.GetComponent<RectTransform>();

            originalValue = target != null ? target.localPosition : Vector3.zero;
        }

        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            if (target == null)
                target = gameObject.GetComponent<RectTransform>();

            if (target != null)
                return DOTween.To(
                    () => target.localPosition,
                    x => target.localPosition = x,
                    originalValue,
                    0f // duración temporal
                ).Pause();
            Debug.LogWarning($"MovementSwitchAction: RectTransform not found on {gameObject.name}");
            return null;
        }


        /// <summary>
        ///     Gets the end position for hiding animation based on the configured hiding position.
        /// </summary>
        /// <returns>The calculated end position for hiding the element.</returns>
        private Vector3 GetEndPosition()
        {
            if (target == null) return Vector3.zero;

            var rect = target.rect;
            var pivot = target.pivot;
            var localPosition = target.localPosition;

            return hidingPosition switch
            {
                Position.Top => new Vector3(
                    originalValue.x,
                    Screen.height * 0.5f + rect.height * pivot.y,
                    originalValue.z),
                Position.Bottom => new Vector3(
                    originalValue.x,
                    -Screen.height * 0.5f - rect.height * (1 - pivot.y),
                    originalValue.z),
                Position.Left => new Vector3(
                    -Screen.width * 0.5f - rect.width * (1 - pivot.x) * scaleCorrection,
                    localPosition.y,
                    localPosition.z),
                Position.Right => new Vector3(
                    Screen.width * 0.5f + rect.width * pivot.x,
                    localPosition.y,
                    localPosition.z),
                _ => Vector3.zero
            };
        }

        #endregion
    }
}
#endif
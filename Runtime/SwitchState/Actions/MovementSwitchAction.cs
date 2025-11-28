#if STATE_BEHAVIOR_ENABLED

using System;
using DG.Tweening;
using LanguageExt;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SwitchState.Actions
{
    /// <summary>
    ///     Represents the position to the right where an element can be hidden.
    /// </summary>
    public enum Position
    {
        /// <summary>
        ///     Specifies the top position within the enum, indicating that the element should be hidden at the top of the screen.
        /// </summary>
        Top,

        /// <summary>
        ///     Represents the bottom position where an element can be hidden.
        /// </summary>
        Bottom,

        /// <summary>
        ///     Specifies the 'Left' position, representing the element being hidden to the left side of the screen.
        /// </summary>
        Left,

        /// <summary>
        ///     Specifies the right position within the enum, indicating that the element should be hidden at the right of the
        ///     screen.
        /// </summary>
        Right
    }

    /// <summary>
    ///     Switch action that animates RectTransform movement for show and hide transitions.
    ///     Moves elements in and out of screen based on configured hiding position.
    /// </summary>
    [Serializable]
    public class MovementSwitchAction : SwitchAction<RectTransform, Vector3>
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


        protected override Option<Vector3> GetEndValue() => GetEndPosition();
        
        #region Methods

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

#if UNITY_EDITOR
        /// <inheritdoc />
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<ActionDataMovementSwitch>();
        }
#endif

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
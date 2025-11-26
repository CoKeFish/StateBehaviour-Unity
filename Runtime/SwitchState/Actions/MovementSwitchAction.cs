#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
using Marmary.StateBehavior.Core;
using Marmary.StateBehavior.SwitchState;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Marmary.StateBehavior.SwitchState.Actions
{
    /// <summary>
    ///     Enum representing the positions where an element can be hidden.
    /// </summary>
    public enum Position
    {
        Top,
        Bottom,
        Left,
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
        [LabelWidth(140)]
        [Tooltip("Scale correction factor applied to width calculations for left/right hiding.")]
        public float scaleCorrection = 1f;

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override void InitializeStartValue(GameObject gameObject)
        {
            if (target == null)
                target = gameObject.GetComponent<RectTransform>();

            OriginalValue = target != null ? target.localPosition : Vector3.zero;
        }

        /// <inheritdoc />
        protected override Tweener CreateTweener(GameObject gameObject)
        {
            if (target == null)
                target = gameObject.GetComponent<RectTransform>();

            if (target == null)
            {
                Debug.LogWarning($"MovementSwitchAction: RectTransform not found on {gameObject.name}");
                return null;
            }

            return DOTween.To(
                () => target.localPosition,
                x => target.localPosition = x,
                OriginalValue,
                0f // duración temporal
            ).Pause();
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
                    OriginalValue.x,
                    Screen.height * 0.5f + rect.height * pivot.y,
                    OriginalValue.z),
                Position.Bottom => new Vector3(
                    OriginalValue.x,
                    -Screen.height * 0.5f - rect.height * (1 - pivot.y),
                    OriginalValue.z),
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

        /// <summary>
        ///     Sets the behavior action for the specified state and applies the corresponding data.
        ///     Overridden to handle dynamic end position calculation for Hide state.
        /// </summary>
        /// <param name="state">The state for which the behavior action is to be configured.</param>
        public override void Set(SwitchState state)
        {
            if (Tweener == null) return;

            var stateWrapper = data.StateActionDataContainers[state];
            var stateData = stateWrapper.BehaviorActionData;
            
            BehaviorActionFactory.Set(stateWrapper.behaviorActionType, Tweener);

            // For Hide state, we need to calculate the end position dynamically
            // For Show state, use base implementation which handles useOrigin correctly
            if (state == SwitchState.Hide)
            {
                // Calculate dynamic end position based on hidingPosition
                var endPosition = GetEndPosition();
                
                // Apply configuration manually for Hide state
                if (stateData is ActionDataSimpleState<Vector3> simpleState)
                {
                    var delay = simpleState.useCustomDelay ? simpleState.customDelay : 0f;
                    Tweener
                        .SetEase(simpleState.easeShow)
                        .SetDelay(delay)
                        .ChangeEndValue(endPosition, simpleState.duration, true)
                        .Restart();
                }
                else
                {
                    // Fallback if not ActionDataSimpleState
                    Tweener.ChangeEndValue(endPosition, 0.5f, true).Restart();
                }
            }
            else
            {
                // For Show state, use base implementation
                base.Set(state);
            }
        }

        /// <summary>
        ///     Applies the behaviour for the supplied state instantly without animation.
        ///     Overridden to handle dynamic end position calculation for Hide state.
        /// </summary>
        /// <param name="state">State to activate.</param>
        public override void InstantSet(SwitchState state)
        {
            if (Tweener == null) return;

            BehaviorActionFactory.Set(BehaviorActionTypes.Instant, Tweener);

            // For Hide state, calculate end position dynamically and apply instantly
            if (state == SwitchState.Hide)
            {
                var endPosition = GetEndPosition();
                if (target != null)
                {
                    target.localPosition = endPosition;
                }
            }
            else
            {
                // For Show state, use base implementation
                base.InstantSet(state);
            }
        }

        #endregion

#if UNITY_EDITOR
        /// <inheritdoc />
        protected override ScriptableObject CreateInstanceScriptableObject()
        {
            return ScriptableObject.CreateInstance<ActionDataMovementSwitch>();
        }
#endif
    }
}
#endif

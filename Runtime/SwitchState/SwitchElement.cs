#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Marmary.StateBehavior.Core;
using UnityEngine;

namespace Marmary.StateBehavior.SwitchState
{
    /// <summary>
    ///     Base component that orchestrates switch animations for show and hide transitions.
    /// </summary>
    public abstract class SwitchElement : Element<SwitchState>
    {
        #region Serialized Fields

        /// <summary>
        ///     Default delay before hiding animation starts.
        /// </summary>
        [SerializeField] public float defaultHideAfter;

        /// <summary>
        ///     Default duration for hiding animation.
        /// </summary>
        [SerializeField] public float defaultHidingDuration = 0.5f;

        /// <summary>
        ///     Default delay before showing animation starts.
        /// </summary>
        [SerializeField] public float defaultShowAfter;

        /// <summary>
        ///     Default duration for showing animation.
        /// </summary>
        [SerializeField] public float defaultShowingDuration = 0.5f;

        #endregion

        #region Fields

        private SwitchStateMachine _stateMachine;

        #endregion

        #region Unity Event Functions

        /// <summary>
        ///     Builds the switch state machine.
        /// </summary>
        protected virtual void Awake()
        {
            _stateMachine = new SwitchStateMachine(SwitchState.Show, gameObject, actions, this);
        }



        #endregion

        #region Methods

        /// <summary>
        ///     Fires the show trigger.
        /// </summary>
        public void OnShow()
        {
            _stateMachine?.FireTrigger(SwitchTrigger.OnShow);
        }

        /// <summary>
        ///     Fires the hide trigger.
        /// </summary>
        public void OnHide()
        {
            _stateMachine?.FireTrigger(SwitchTrigger.OnHide);
        }

        /// <summary>
        ///     Alias for <see cref="OnShow" /> for semantic clarity when used externally.
        ///     Note: This triggers the state machine immediately. For async version with delays, use <see cref="Show()" />.
        /// </summary>
        public void ShowSync() => OnShow();

        /// <summary>
        ///     Alias for <see cref="OnHide" /> for semantic clarity when used externally.
        ///     Note: This triggers the state machine immediately. For async version with delays, use <see cref="Hide()" />.
        /// </summary>
        public void HideSync() => OnHide();

        /// <summary>
        ///     Hides the element with custom delays per action.
        /// </summary>
        public async UniTask Hide()
        {
            if (actions == null || actions.Count == 0)
            {
                if (Events.ContainsKey(SwitchState.Hide))
                    Events[SwitchState.Hide]?.Invoke();
                return;
            }

            var tasks = new List<UniTask>();

            foreach (var action in actions)
            {
                var (useCustomDelay, delay, duration) = GetHideTiming(action);
                tasks.Add(HideWithCustomDelay(action, delay, duration));
            }

            await UniTask.WhenAll(tasks);

            if (Events.ContainsKey(SwitchState.Hide))
                Events[SwitchState.Hide]?.Invoke();

            // Local function para el delay personalizado por acción
            async UniTask HideWithCustomDelay(IStateContract<SwitchState> action, float delay, float duration)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay));

                // Trigger the hide state
                action.Set(SwitchState.Hide);

                // Wait for tween completion if we have access to it
                var tweener = GetTweenerFromAction(action);
                if (tweener != null && tweener.IsActive())
                {
                    await tweener.AsyncWaitForCompletion().AsUniTask();
                }
                else
                {
                    // Fallback: wait for duration if we can't access tweener
                    await UniTask.Delay(TimeSpan.FromSeconds(duration));
                }
            }
        }

        /// <summary>
        ///     Shows the element with custom delays per action.
        /// </summary>
        public async UniTask Show()
        {
            if (actions == null || actions.Count == 0)
            {
                if (Events.ContainsKey(SwitchState.Show))
                    Events[SwitchState.Show]?.Invoke();
                return;
            }

            var tasks = new List<UniTask>();

            foreach (var action in actions)
            {
                var (useCustomDelay, delay, duration) = GetShowTiming(action);
                tasks.Add(ShowWithCustomDelay(action, delay, duration));
            }

            await UniTask.WhenAll(tasks);

            if (Events.ContainsKey(SwitchState.Show))
                Events[SwitchState.Show]?.Invoke();

            // Local function para el delay personalizado por acción
            async UniTask ShowWithCustomDelay(IStateContract<SwitchState> action, float delay, float duration)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay));

                // Trigger the show state
                action.Set(SwitchState.Show);

                // Wait for tween completion if we have access to it
                var tweener = GetTweenerFromAction(action);
                if (tweener != null && tweener.IsActive())
                {
                    await tweener.AsyncWaitForCompletion().AsUniTask();
                }
                else
                {
                    // Fallback: wait for duration if we can't access tweener
                    await UniTask.Delay(TimeSpan.FromSeconds(duration));
                }
            }
        }

        /// <summary>
        ///     Instantly sets the element to the hide state without animation.
        /// </summary>
        public void InstantHide()
        {
            if (actions == null) return;
            foreach (var action in actions)
                action.InstantSet(SwitchState.Hide);

            if (Events.ContainsKey(SwitchState.Hide))
                Events[SwitchState.Hide]?.Invoke();
        }

        /// <summary>
        ///     Instantly sets the element to the show state without animation.
        /// </summary>
        public void InstantShow()
        {
            if (actions == null) return;
            foreach (var action in actions)
                action.InstantSet(SwitchState.Show);

            if (Events.ContainsKey(SwitchState.Show))
                Events[SwitchState.Show]?.Invoke();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Gets the hide timing (delay and duration) for an action.
        /// </summary>
        /// <param name="action">The action to get timing for.</param>
        /// <returns>Tuple containing (useCustomDelay, delay, duration).</returns>
        private (bool useCustomDelay, float delay, float duration) GetHideTiming(IStateContract<SwitchState> action)
        {
            // Use reflection to access the data property
            var actionType = action.GetType();
            var dataProperty = actionType.GetProperty("data", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (dataProperty == null) 
                return (false, defaultHideAfter, defaultHidingDuration);

            var data = dataProperty.GetValue(action);
            if (data == null) 
                return (false, defaultHideAfter, defaultHidingDuration);

            // Get StateActionDataContainers dictionary
            var containersProperty = data.GetType().GetProperty("StateActionDataContainers");
            if (containersProperty == null) 
                return (false, defaultHideAfter, defaultHidingDuration);

            var containers = containersProperty.GetValue(data) as System.Collections.IDictionary;
            if (containers == null || !containers.Contains(SwitchState.Hide)) 
                return (false, defaultHideAfter, defaultHidingDuration);

            var stateWrapper = containers[SwitchState.Hide];
            if (stateWrapper == null) 
                return (false, defaultHideAfter, defaultHidingDuration);

            // Get BehaviorActionData
            var behaviorDataProperty = stateWrapper.GetType().GetProperty("BehaviorActionData");
            if (behaviorDataProperty == null) 
                return (false, defaultHideAfter, defaultHidingDuration);

            var behaviorData = behaviorDataProperty.GetValue(stateWrapper);
            if (behaviorData == null) 
                return (false, defaultHideAfter, defaultHidingDuration);

            // Check if it's ActionDataSimpleState
            var behaviorDataType = behaviorData.GetType();
            if (!behaviorDataType.IsGenericType || 
                behaviorDataType.GetGenericTypeDefinition() != typeof(ActionDataSimpleState<>)) 
                return (false, defaultHideAfter, defaultHidingDuration);

            // Get useCustomDelay, customDelay, and duration properties
            var useCustomDelayProp = behaviorDataType.GetProperty("useCustomDelay");
            var customDelayProp = behaviorDataType.GetProperty("customDelay");
            var durationProp = behaviorDataType.GetProperty("duration");

            if (useCustomDelayProp == null || customDelayProp == null || durationProp == null)
                return (false, defaultHideAfter, defaultHidingDuration);

            var useCustomDelay = (bool)useCustomDelayProp.GetValue(behaviorData);
            var customDelay = (float)customDelayProp.GetValue(behaviorData);
            var duration = (float)durationProp.GetValue(behaviorData);

            var delay = useCustomDelay ? customDelay : defaultHideAfter;
            return (useCustomDelay, delay, duration);
        }

        /// <summary>
        ///     Gets the show timing (delay and duration) for an action.
        /// </summary>
        /// <param name="action">The action to get timing for.</param>
        /// <returns>Tuple containing (useCustomDelay, delay, duration).</returns>
        private (bool useCustomDelay, float delay, float duration) GetShowTiming(IStateContract<SwitchState> action)
        {
            // Use reflection to access the data property
            var actionType = action.GetType();
            var dataProperty = actionType.GetProperty("data", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (dataProperty == null) 
                return (false, defaultShowAfter, defaultShowingDuration);

            var data = dataProperty.GetValue(action);
            if (data == null) 
                return (false, defaultShowAfter, defaultShowingDuration);

            // Get StateActionDataContainers dictionary
            var containersProperty = data.GetType().GetProperty("StateActionDataContainers");
            if (containersProperty == null) 
                return (false, defaultShowAfter, defaultShowingDuration);

            var containers = containersProperty.GetValue(data) as System.Collections.IDictionary;
            if (containers == null || !containers.Contains(SwitchState.Show)) 
                return (false, defaultShowAfter, defaultShowingDuration);

            var stateWrapper = containers[SwitchState.Show];
            if (stateWrapper == null) 
                return (false, defaultShowAfter, defaultShowingDuration);

            // Get BehaviorActionData
            var behaviorDataProperty = stateWrapper.GetType().GetProperty("BehaviorActionData");
            if (behaviorDataProperty == null) 
                return (false, defaultShowAfter, defaultShowingDuration);

            var behaviorData = behaviorDataProperty.GetValue(stateWrapper);
            if (behaviorData == null) 
                return (false, defaultShowAfter, defaultShowingDuration);

            // Check if it's ActionDataSimpleState
            var behaviorDataType = behaviorData.GetType();
            if (!behaviorDataType.IsGenericType || 
                behaviorDataType.GetGenericTypeDefinition() != typeof(ActionDataSimpleState<>)) 
                return (false, defaultShowAfter, defaultShowingDuration);

            // Get useCustomDelay, customDelay, and duration properties
            var useCustomDelayProp = behaviorDataType.GetProperty("useCustomDelay");
            var customDelayProp = behaviorDataType.GetProperty("customDelay");
            var durationProp = behaviorDataType.GetProperty("duration");

            if (useCustomDelayProp == null || customDelayProp == null || durationProp == null)
                return (false, defaultShowAfter, defaultShowingDuration);

            var useCustomDelay = (bool)useCustomDelayProp.GetValue(behaviorData);
            var customDelay = (float)customDelayProp.GetValue(behaviorData);
            var duration = (float)durationProp.GetValue(behaviorData);

            var delay = useCustomDelay ? customDelay : defaultShowAfter;
            return (useCustomDelay, delay, duration);
        }

        /// <summary>
        ///     Gets the Tweener from an action if possible.
        /// </summary>
        /// <param name="action">The action to get the tweener from.</param>
        /// <returns>The Tweener instance or null if not accessible.</returns>
        private Tweener GetTweenerFromAction(IStateContract<SwitchState> action)
        {
            // Use reflection to access protected Tweener field
            var actionType = action.GetType();
            var tweenerField = actionType.BaseType?.GetField("Tweener", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return tweenerField?.GetValue(action) as Tweener;
        }

        #endregion
    }
}
#endif


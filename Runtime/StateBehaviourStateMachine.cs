using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Represents an abstract state machine that operates with states and triggers defined as enumerations.
    /// </summary>
    /// <typeparam name="TState">The enumeration type representing the states.</typeparam>
    /// <typeparam name="TTrigger">The enumeration type representing the triggers.</typeparam>
    public abstract class StateBehaviourStateMachine<TState, TTrigger> : StateMachineBase<TState, TTrigger>
        where TState : Enum
        where TTrigger : Enum
    {
        #region Fields

        /// <summary>
        ///     The animation element that handles events for selection states.
        /// </summary>
        private readonly Element<TState, TTrigger> _selectableElement;

        /// <summary>
        ///     Provides timing configurations and functionality to control execution delays
        ///     in the state behavior state machine.
        /// </summary>
        private readonly TimeWrapper _timeWrapper;

        /// <summary>
        ///     List of actions to execute on state changes.
        /// </summary>
        private readonly List<IStateContract<TState>> _actions;

        /// <summary>
        ///     Collection of asynchronous tasks generated while executing the current state.
        /// </summary>
        private readonly List<UniTask> _executionTasks = new();

        #endregion

        #region Properties

        /// <summary>
        ///     Executes all actions or events configured for the current state of the state machine.
        /// </summary>
        public bool ShouldExecuteInstantly;

        #endregion

        #region Constructors and Injected

        protected StateBehaviourStateMachine(TState initialState,
            List<IStateContract<TState>> actions,
            Element<TState, TTrigger> selectableElement,
            TimeWrapper timeWrapper,
            bool executeInstantly = false)
            : base(initialState)
        {
            _actions = actions;
            _selectableElement = selectableElement;
            _timeWrapper = timeWrapper;
            ShouldExecuteInstantly = executeInstantly;

            ConfigureStateMachine();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fires a trigger forcing the next state execution to run instantly.
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        public void FireTriggerInstant(TTrigger trigger)
        {
            var previousInstantSetting = ShouldExecuteInstantly;
            ShouldExecuteInstantly = true;
            stateMachine.Fire(trigger);
            ShouldExecuteInstantly = previousInstantSetting;
        }

        /// <summary>
        ///     Fires a trigger asynchronously, ensuring that the next state execution is not forced to run instantly.
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        public void FireTriggerAsync(TTrigger trigger)
        {
            var previousInstantSetting = ShouldExecuteInstantly;
            ShouldExecuteInstantly = false;
            stateMachine.Fire(trigger);
            ShouldExecuteInstantly = previousInstantSetting;
        }


        /// <summary>
        ///     Retrieves a task that completes when all registered execution tasks finish.
        /// </summary>
        /// <returns>A UniTask representing the completion of the current execution batch.</returns>
        public UniTask WhenExecutionCompletes()
        {
            return _executionTasks.Count == 0 ? UniTask.CompletedTask : UniTask.WhenAll(_executionTasks);
        }


        /// <summary>
        ///     Executes the configured actions and events for the current state.
        /// </summary>
        protected void ExecuteActions()
        {
            ResetExecutionTasks();

            if (_actions.IsNullOrEmpty()) return;

            var currentState = stateMachine.State;

            if (ShouldExecuteInstantly)
            {
                InstantProcess(currentState);
                return;
            }

            AnimationProcess(currentState, _timeWrapper);
        }


        /// <summary>
        ///     Clears the list of tasks registered for the current execution batch.
        /// </summary>
        private void ResetExecutionTasks()
        {
            _executionTasks.Clear();
        }

        /// <summary>
        ///     Adds a task to be tracked as part of the current execution batch.
        /// </summary>
        /// <param name="task">Task created while executing the state.</param>
        private void AddExecutionTask(UniTask task)
        {
            _executionTasks.Add(task);
        }

        /// <summary>
        ///     Executes animation tasks for the specified state based on the associated actions.
        /// </summary>
        /// <param name="currentState">The current state of the state machine for which animations will be processed.</param>
        /// <param name="timeWrapper">An instance of the time wrapper used to manage timing during animation execution.</param>
        private void AnimationProcess(TState currentState, TimeWrapper timeWrapper)
        {
            var tasks = new List<UniTask>(_actions.Count);
            tasks.AddRange(Enumerable.Select(_actions, action => action.Set(currentState, timeWrapper)));

            var finalTask = AwaitActionsThenInvokeEvents(tasks, currentState);
            AddExecutionTask(finalTask);
        }

        /// <summary>
        ///     Executes instant processing for the specified state by applying all associated actions
        ///     and invoking relevant state events.
        /// </summary>
        /// <param name="currentState">The current state to process instantly.</param>
        private void InstantProcess(TState currentState)
        {
            foreach (var action in _actions) action.InstantSet(currentState);

            TriggerStateEvents(currentState);
            AddExecutionTask(UniTask.CompletedTask);
        }

        /// <summary>
        ///     Triggers the associated events for the specified state in the selectable element.
        /// </summary>
        /// <param name="state">The current state for which the events should be triggered.</param>
        private void TriggerStateEvents(TState state)
        {
            if (_selectableElement.Events.ContainsKey(state))
                _selectableElement.Events[state]?.Invoke();
        }

        /// <summary>
        ///     Awaits the completion of the provided asynchronous tasks and then triggers state-specific events.
        /// </summary>
        /// <param name="tasks">A list of asynchronous tasks to be awaited.</param>
        /// <param name="state">The current state for which events will be triggered after task completion.</param>
        /// <returns>A UniTask that completes once all provided tasks have been awaited and the state events have been triggered.</returns>
        private async UniTask AwaitActionsThenInvokeEvents(IEnumerable<UniTask> tasks, TState state)
        {
            await UniTask.WhenAll(tasks);
            TriggerStateEvents(state);
        }

        #endregion
    }
}
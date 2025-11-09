#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;
using Marmary.StateBehavior.Core;
using Sirenix.Utilities;
using UnityEngine;

namespace Marmary.StateBehavior.SwitchState
{
    /// <summary>
    /// Defines a state machine responsible for managing state transitions,
    /// supporting operations to switch states dynamically while executing
    /// corresponding behaviors or animations.
    /// </summary>
    internal class SwitchStateMachine : StateMachineBase<SwitchState, SwitchTrigger>
    {
        #region Fields

        /// <summary>
        /// Stores a collection of state-specific actions that implement the IStateContract interface,
        /// allowing the state machine to execute setup and transition logic for each state.
        /// </summary>
        private readonly List<IStateContract<SwitchState>> _actions;

        /// <summary>
        /// Represents the SwitchElement instance associated with the state machine,
        /// which controls UI element show and hide transitions and manages related animations.
        /// </summary>
        private readonly SwitchElement _switchElement;

        #endregion

        #region Constructors and Injected

        /// <summary>
        /// Initializes a switch state machine with a specified initial state and associated configurations.
        /// </summary>
        /// <param name="initialState">The starting state for the state machine.</param>
        /// <param name="gameObject">The GameObject associated with this state machine.</param>
        /// <param name="actions">A collection of actions triggered during state transitions.</param>
        /// <param name="switchElement">The interactive element that operates with the state machine.</param>
        public SwitchStateMachine(SwitchState initialState,
            GameObject gameObject,
            List<IStateContract<SwitchState>> actions,
            SwitchElement switchElement) : base(initialState)
        {
            _switchElement = switchElement;
            _actions = actions;

            ConfigureStateMachine();

            if (_actions.IsNullOrEmpty()) return;

            foreach (var action in _actions) 
                action.Setup(gameObject);
        }

        #endregion
        

        #region Methods

        /// <inheritdoc />
        protected sealed override void ConfigureStateMachine()
        {
            StateMachine.Configure(SwitchState.Show)
                .Permit(SwitchTrigger.OnHide, SwitchState.Hide)
                .PermitReentry(SwitchTrigger.OnShow)
                .OnEntry(ExecuteActions);

            StateMachine.Configure(SwitchState.Hide)
                .Permit(SwitchTrigger.OnShow, SwitchState.Show)
                .PermitReentry(SwitchTrigger.OnHide)
                .OnEntry(ExecuteActions);
        }
        
        #endregion

        #region Helpers

        /// <summary>
        /// Executes all actions configured for the current state of the state machine.
        /// </summary>
        protected override void ExecuteActions()
        {
            if (!_actions.IsNullOrEmpty())
                foreach (var action in _actions)
                    action.Set(StateMachine.State);

            if (_switchElement.Events.ContainsKey(StateMachine.State))
                _switchElement.Events[StateMachine.State]?.Invoke();
        }

        #endregion
    }
}
#endif


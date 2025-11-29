#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SwitchState
{
    /// <summary>
    ///     Defines a state machine responsible for managing state transitions,
    ///     supporting operations to switch states dynamically while executing
    ///     corresponding behaviors or animations.
    /// </summary>
    internal class SwitchStateMachine : StateBehaviourStateMachine<SwitchState, SwitchTrigger>
    {
        #region Constructors and Injected

        /// <summary>
        ///     Initializes a switch state machine with a specified initial state and associated configurations.
        /// </summary>
        /// <param name="initialState">The starting state for the state machine.</param>
        /// <param name="gameObject">The GameObject associated with this state machine.</param>
        /// <param name="actions">A collection of actions triggered during state transitions.</param>
        /// <param name="switchElement">The interactive element that operates with the state machine.</param>
        public SwitchStateMachine(SwitchState initialState,
            GameObject gameObject,
            List<IStateContract<SwitchState>> actions,
            SwitchElement switchElement) : base(initialState, gameObject, actions, switchElement)
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        protected sealed override void ConfigureStateMachine()
        {
            stateMachine.Configure(SwitchState.Show)
                .Permit(SwitchTrigger.OnHide, SwitchState.Hide)
                .PermitReentry(SwitchTrigger.OnShow)
                .OnEntry(ExecuteActions);

            stateMachine.Configure(SwitchState.Hide)
                .Permit(SwitchTrigger.OnShow, SwitchState.Show)
                .PermitReentry(SwitchTrigger.OnHide)
                .OnEntry(ExecuteActions);
        }

        #endregion
    }
}
#endif
#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;
using UnityEngine.Events;

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
        /// Represents a state machine specifically designed to manage transitions and behavior
        /// associated with switch states, integrating state-specific actions, events, and time-based logic.
        /// </summary>
        /// <param name="initialState">The initial state of the switch state machine.</param>
        /// <param name="actions">A collection of state-specific contracts defining behaviors during transitions.</param>
        /// <param name="events">A mapping of switch states to corresponding UnityEvents that should be triggered.</param>
        /// <param name="timeWrapper">Instance used for managing and handling time-based operations.</param>
        /// <param name="executeInstantly">
        /// A flag indicating whether actions associated with the initial state should be executed immediately.
        /// </param>
        public SwitchStateMachine(SwitchState initialState,
            List<IStateContract<SwitchState>> actions,
            Dictionary<SwitchState, UnityEvent> events,
            TimeWrapper timeWrapper,
            bool executeInstantly)
            : base(initialState, actions, events, timeWrapper, executeInstantly)
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
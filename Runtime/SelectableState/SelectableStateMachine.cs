#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;
using UnityEngine.Events;

namespace Marmary.StateBehavior.Runtime.SelectableState
{
    /// <summary>
    ///     State machine for managing the state transitions and actions of a selectable UI element.
    /// </summary>
    internal class SelectableStateMachine : StateBehaviourStateMachine<SelectableState, SelectableTrigger>
    {
        #region Fields

        /// <summary>
        ///     Represents an event that is invoked when a selectable UI element is clicked.
        ///     This variable is used to store a UnityEvent that triggers actions associated with the click event.
        /// </summary>
        private readonly UnityEvent _onClick;

        #endregion

        #region Constructors and Injected

        /// <summary>
        ///     Represents a state machine responsible for handling selectable states and their transitions.
        /// </summary>
        /// <param name="initialState">The initial state assigned to the state machine.</param>
        /// <param name="actions">A collection of actions to execute during state changes.</param>
        /// <param name="selectableElement">The UI element that triggers or reacts to state transitions.</param>
        /// <param name="timeWrapper">The time-related utility that aids in managing timing during transitions.</param>
        /// <param name="onClick">The UnityEvent that is invoked when a selectable element is clicked.</param>
        public SelectableStateMachine(SelectableState initialState,
            List<IStateContract<SelectableState>> actions,
            SelectableElement selectableElement,
            TimeWrapper timeWrapper,
            UnityEvent onClick)
            : base(initialState, actions, selectableElement, timeWrapper)
        {
            _onClick = onClick;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Configures the state machine with state transitions and entry actions.
        /// </summary>
        /// <inheritdoc />
        protected sealed override void ConfigureStateMachine()
        {
            stateMachine.Configure(SelectableState.Normal)
                .Ignore(SelectableTrigger.PointerClick)
                .Ignore(SelectableTrigger.UnPressed)
                .Ignore(SelectableTrigger.PointerDown)
                .Ignore(SelectableTrigger.PointerUp)
                .Permit(SelectableTrigger.PointerEnter, SelectableState.Highlighted)
                .Permit(SelectableTrigger.Select, SelectableState.Highlighted)
                .OnEntry(ExecuteActions);

            stateMachine.Configure(SelectableState.Highlighted)
                .PermitReentry(SelectableTrigger.PointerClick)
                .Permit(SelectableTrigger.PointerExit, SelectableState.Normal)
                .Permit(SelectableTrigger.Deselect, SelectableState.Normal)
                .Permit(SelectableTrigger.PointerDown, SelectableState.PressedInside)
                .Permit(SelectableTrigger.Submit, SelectableState.PressedInside)
                .Permit(SelectableTrigger.Cancel, SelectableState.Normal)
                .OnEntryFrom(SelectableTrigger.PointerClick, OnClick)
                .OnEntryFrom(SelectableTrigger.UnPressed, OnClick)
                .OnEntry(ExecuteActions);

            stateMachine.Configure(SelectableState.PressedInside)
                .Ignore(SelectableTrigger.Cancel)
                .Permit(SelectableTrigger.PointerExit, SelectableState.PressedOutside)
                .Permit(SelectableTrigger.Deselect, SelectableState.PressedOutside)
                .Permit(SelectableTrigger.PointerUp, SelectableState.Highlighted)
                .Permit(SelectableTrigger.UnPressed, SelectableState.Highlighted)
                .OnEntry(ExecuteActions);

            // You can trigger your click event in OnExit or in a specific handler here

            stateMachine.Configure(SelectableState.PressedOutside)
                .Permit(SelectableTrigger.Cancel, SelectableState.Normal)
                .Permit(SelectableTrigger.UnPressed, SelectableState.Normal)
                .Permit(SelectableTrigger.PointerEnter, SelectableState.PressedInside)
                .Permit(SelectableTrigger.Select, SelectableState.PressedInside)
                .Permit(SelectableTrigger.PointerUp, SelectableState.Normal) // Released outside, no click
                .OnEntry(ExecuteActions);
        }

        #endregion

        #region Event Functions

        /// <summary>
        ///     Invokes the associated UnityEvent for the selectable element and executes state-related actions.
        /// </summary>
        private void OnClick()
        {
            _onClick.Invoke();
            ExecuteActions();
        }

        #endregion
    }
}
#endif
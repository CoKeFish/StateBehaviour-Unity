#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SelectableState
{
    /// <summary>
    ///     State machine for managing the state transitions and actions of a selectable UI element.
    /// </summary>
    internal class SelectableStateMachine : StateBehaviourStateMachine<SelectableState, SelectableTrigger>
    {
        #region Constructors and Injected

        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectableStateMachine" /> class.
        /// </summary>
        /// <param name="initialState">The initial state of the state machine.</param>
        /// <param name="gameObject">The GameObject associated with this state machine.</param>
        /// <param name="actions">The list of actions to execute on state changes.</param>
        /// <param name="selectableElement">The animation element for selection events.</param>
        public SelectableStateMachine(SelectableState initialState,
            GameObject gameObject,
            List<IStateContract<SelectableState>> actions,
            SelectableElement selectableElement) : base(initialState, gameObject, actions, selectableElement)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Configures the state machine with state transitions and entry actions.
        /// </summary>
        /// <inheritdoc />
        protected sealed override void ConfigureStateMachine()
        {
            StateMachine.Configure(SelectableState.Normal)
                .Ignore(SelectableTrigger.PointerClick)
                .Ignore(SelectableTrigger.UnPressed)
                .Permit(SelectableTrigger.PointerEnter, SelectableState.Highlighted)
                .Permit(SelectableTrigger.Select, SelectableState.Highlighted)
                .OnEntry(ExecuteActions);

            StateMachine.Configure(SelectableState.Highlighted)
                .PermitReentry(SelectableTrigger.PointerClick)
                .Permit(SelectableTrigger.PointerExit, SelectableState.Normal)
                .Permit(SelectableTrigger.Deselect, SelectableState.Normal)
                .Permit(SelectableTrigger.PointerDown, SelectableState.PressedInside)
                .Permit(SelectableTrigger.Submit, SelectableState.PressedInside)
                .Permit(SelectableTrigger.Cancel, SelectableState.Normal)
                .OnEntryFrom(SelectableTrigger.PointerClick, ExecuteActions)
                .OnEntryFrom(SelectableTrigger.UnPressed, ExecuteActions)
                .OnEntry(ExecuteActions);

            StateMachine.Configure(SelectableState.PressedInside)
                .Ignore(SelectableTrigger.Cancel)
                .Permit(SelectableTrigger.PointerExit, SelectableState.PressedOutside)
                .Permit(SelectableTrigger.Deselect, SelectableState.PressedOutside)
                .Permit(SelectableTrigger.PointerUp, SelectableState.Highlighted)
                .Permit(SelectableTrigger.UnPressed, SelectableState.Highlighted)
                .OnEntry(ExecuteActions);

            // You can trigger your click event in OnExit or in a specific handler here

            StateMachine.Configure(SelectableState.PressedOutside)
                .Permit(SelectableTrigger.Cancel, SelectableState.Normal)
                .Permit(SelectableTrigger.UnPressed, SelectableState.Normal)
                .Permit(SelectableTrigger.PointerEnter, SelectableState.PressedInside)
                .Permit(SelectableTrigger.Select, SelectableState.PressedInside)
                .Permit(SelectableTrigger.PointerUp, SelectableState.Normal) // Released outside, no click
                .OnEntry(ExecuteActions);
        }
        

        #endregion
    }
}
#endif
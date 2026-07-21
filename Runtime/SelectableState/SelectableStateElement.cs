#if STATE_BEHAVIOR_ENABLED
using UnityEngine.Events;

namespace Marmary.StateBehavior.Runtime.SelectableState
{
    /// <summary>
    ///     Base class for elements animated by the <see cref="SelectableState" /> machine,
    ///     independent of where the input comes from. Concrete elements provide the input
    ///     source: <see cref="SelectableElement" /> listens to uGUI events next to a
    ///     Selectable, while widget adapters (e.g. virtualized list items) can feed the
    ///     triggers from their own event pipelines without requiring a Selectable.
    /// </summary>
    public abstract class SelectableStateElement : Element<SelectableState, SelectableTrigger>
    {
        #region Serialized Fields

        /// <summary>
        ///     Represents the event triggered when the UI element is clicked.
        ///     This event is invoked to handle click interactions and associated logic for the selectable element.
        /// </summary>
        public UnityEvent onClick = new();

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the element by creating and configuring the internal SelectableStateMachine instance.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            stateMachine = new SelectableStateMachine(SelectableState.Normal, actions, Events, time, executeInstantly,
                onClick);
        }

        /// <summary>
        ///     Fires the trigger only when the machine can handle it in its current state,
        ///     silently dropping it otherwise. Input sources that can produce redundant
        ///     triggers (e.g. hover and selection both leading to Highlighted) must use this
        ///     instead of <see cref="Element{TState,TTrigger}.TriggerState(TTrigger)" />,
        ///     which throws on unhandled triggers.
        /// </summary>
        /// <param name="trigger">Trigger to fire.</param>
        /// <returns>Whether the trigger was fired.</returns>
        protected bool TryTriggerState(SelectableTrigger trigger)
        {
            if (stateMachine == null || !stateMachine.CanFire(trigger)) return false;

            TriggerState(trigger);
            return true;
        }

        /// <summary>
        ///     Returns the state machine to <see cref="SelectableState.Normal" /> instantly,
        ///     whatever the current state. Meant for recycled instances (object pools) so a
        ///     reused element does not inherit the Highlighted/Pressed state of its previous
        ///     use. No-op while the machine does not exist yet.
        /// </summary>
        protected void ResetStateInstant()
        {
            if (stateMachine == null) return;

            // Deselect then Cancel reaches Normal from every state: Highlighted -> Normal
            // (Deselect); PressedInside -> PressedOutside (Deselect) -> Normal (Cancel);
            // PressedOutside -> Normal (Cancel); Normal fires neither.
            if (stateMachine.CanFire(SelectableTrigger.Deselect))
                stateMachine.FireTriggerInstant(SelectableTrigger.Deselect);

            if (stateMachine.CanFire(SelectableTrigger.Cancel))
                stateMachine.FireTriggerInstant(SelectableTrigger.Cancel);
        }

        #endregion
    }
}
#endif
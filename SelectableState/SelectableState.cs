namespace Marmary.StateBehavior.SelectableState
{
// El enum define los estados posibles
    /// <summary>
    ///     Represents the possible states for a selectable UI element.
    /// </summary>
    public enum SelectableState
    {
        /// <summary>
        ///     The default state when the UI element is not interacted with.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     The state when the UI element is highlighted (e.g., hovered or selected).
        /// </summary>
        Highlighted,

        /// <summary>
        ///     The state when the UI element is pressed while the pointer is inside its bounds.
        /// </summary>
        PressedInside,

        /// <summary>
        ///     The state when the UI element is pressed while the pointer is outside its bounds.
        /// </summary>
        PressedOutside
    }

    /// <summary>
    ///     Defines the triggers that can cause state transitions in the SelectableStateMachine.
    /// </summary>
    public enum SelectableTrigger
    {
        /// <summary>
        /// Triggered when the pointer enters the UI element.
        /// </summary>
        PointerEnter,
        /// <summary>
        /// Triggered when the pointer exits the UI element.
        /// </summary>
        PointerExit,
        /// <summary>
        /// Triggered when the pointer is pressed down on the UI element.
        /// </summary>
        PointerDown,
        /// <summary>
        /// Triggered when the UI element is clicked.
        /// </summary>
        PointerClick,
        /// <summary>
        /// Triggered when the UI element is selected.
        /// </summary>
        Select,
        /// <summary>
        /// Triggered when the UI element is deselected.
        /// </summary>
        Deselect,
        /// <summary>
        /// Triggered when the pointer is released over the UI element.
        /// </summary>
        PointerUp,
        /// <summary>
        /// Triggered when a submit action occurs.
        /// </summary>
        Submit,
        /// <summary>
        /// Triggered when a cancel action occurs.
        /// </summary>
        Cancel,
        /// <summary>
        /// Triggered when the pressed state is released.
        /// </summary>
        UnPressed
    }
    
}
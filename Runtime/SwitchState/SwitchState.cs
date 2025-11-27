#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior.Runtime.SwitchState
{
    /// <summary>
    ///     Represents the visibility states for a switchable UI element.
    /// </summary>
    public enum SwitchState
    {
        /// <summary>
        ///     The element is fully visible and ready to play show actions.
        /// </summary>
        Show = 0,

        /// <summary>
        ///     The element is hidden.
        /// </summary>
        Hide
    }

    /// <summary>
    ///     Defines the triggers that control the <see cref="SwitchStateMachine" /> transitions.
    /// </summary>
    public enum SwitchTrigger
    {
        /// <summary>
        ///     Requests the element to transition to the show state.
        /// </summary>
        OnShow,

        /// <summary>
        ///     Requests the element to transition to the hide state.
        /// </summary>
        OnHide
    }
}
#endif
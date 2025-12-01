namespace Marmary.StateBehavior.Runtime.SwitchState.Actions
{
    /// <summary>
    ///     A specialized RectTransform movement action for handling state transitions
    ///     within the SwitchState system.
    /// </summary>
    /// <remarks>
    ///     Inherits from <see cref="RectTransformMovementAction{TState, TActionData}" />
    ///     with <see cref="SwitchState" /> as the state type and
    ///     <see cref="RectTransformMovementDataSwitchAction" /> as the action data type.
    /// </remarks>
    /// <example>
    ///     This class is designed to manage the movement of RectTransform elements
    ///     during transitions between "Show" and "Hide" states in a UI context.
    /// </example>
    public class
        RectTransformMovementSwitchAction : RectTransformMovementAction<SwitchState,
        RectTransformMovementDataSwitchAction>
    {
    }
}
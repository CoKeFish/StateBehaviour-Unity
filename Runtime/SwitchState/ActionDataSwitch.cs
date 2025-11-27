#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior.Runtime.SwitchState
{
    /// <summary>
    ///     Base action data mapping for <see cref="SwitchState" /> driven behaviours.
    /// </summary>
    /// <typeparam name="TValue">Type of value animated by the switch action.</typeparam>
    public abstract class ActionDataSwitch<TValue> : ActionData<SwitchState, TValue>
    {
    }
}
#endif
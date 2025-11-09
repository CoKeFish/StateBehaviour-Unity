#if STATE_BEHAVIOR_ENABLED
using Marmary.StateBehavior.Core;

namespace Marmary.StateBehavior.SwitchState
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


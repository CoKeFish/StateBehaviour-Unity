#if STATE_BEHAVIOR_ENABLED
using Marmary.StateBehavior.Core;

namespace Marmary.StateBehavior.SelectableState
{
    /// <summary>
    ///     Base action data scoped to <see cref="SelectableState"/> values.
    /// </summary>
    /// <typeparam name="TValue">Type of value driven by the selectable action.</typeparam>
    public abstract class ActionDataSelectable<TValue> : ActionData<SelectableState, TValue>
    {
    }
}
#endif
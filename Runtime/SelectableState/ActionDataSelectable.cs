#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior.Runtime.SelectableState
{
    /// <summary>
    ///     Base action data scoped to <see cref="SelectableState" /> values.
    /// </summary>
    /// <typeparam name="TValue">Type of value driven by the selectable action.</typeparam>
    public abstract class ActionDataSelectable<TValue> : ActionData<SelectableState, TValue>
    {
    }
}
#endif
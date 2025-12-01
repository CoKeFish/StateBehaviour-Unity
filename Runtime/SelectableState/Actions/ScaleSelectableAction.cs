using Marmary.StateBehavior.Runtime.Actions;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Represents an action that scales a selectable UI element's RectTransform
    ///     in response to state changes defined in <see cref="SelectableState" />.
    /// </summary>
    /// <remarks>
    ///     This class extends the behavior of <see cref="ScaleAction{SelectableState,ScaleDataSelectableAction}" />
    ///     by introducing state-driven scaling for UI elements.
    /// </remarks>
    public class ScaleSelectableAction : ScaleAction<SelectableState, ScaleDataSelectableAction>
    {
    }
}
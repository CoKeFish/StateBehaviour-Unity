using Marmary.StateBehavior.Runtime.Actions;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Handles the animation of color transitions for an <see cref="UnityEngine.UI.Image" /> component
    ///     based on changes in selectable UI states.
    /// </summary>
    /// <remarks>
    ///     This class extends <see cref="ColorSpriteAction{SelectableState,ColorSpriteDataSelectableAction}" />
    ///     specifically for <see cref="Marmary.StateBehavior.Runtime.SelectableState.SelectableState" /> state transitions.
    ///     It utilizes <see cref="ColorSpriteDataSelectableAction" /> to define color transitions for the states:
    ///     Normal, Highlighted, PressedInside, and PressedOutside.
    /// </remarks>
    /// <seealso cref="ColorSpriteAction{TState, TActionData}" />
    /// <seealso cref="ColorSpriteDataSelectableAction" />
    /// <seealso cref="Marmary.StateBehavior.Runtime.SelectableState.SelectableState" />
    public class ColorSpriteSelectableAction : ColorSpriteAction<SelectableState, ColorSpriteDataSelectableAction>
    {
    }
}
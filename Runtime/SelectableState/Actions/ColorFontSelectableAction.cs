using Marmary.StateBehavior.Runtime.Actions;
using TMPro;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Implements a customizable action for animating the font color of a
    ///     <see cref="TextMeshProUGUI" /> component when the <see cref="SelectableState" />
    ///     transitions occur.
    /// </summary>
    /// <remarks>
    ///     This class extends the functionality of the <see cref="ColorFontAction{TState,TActionData}" />
    ///     abstract class by specializing the state type to <see cref="SelectableState" />
    ///     and the action data to <see cref="ColorFontDataSelectableAction" />. It integrates
    ///     smooth color transitions in response to different selectable states such as
    ///     Normal, Highlighted, PressedInside, and PressedOutside.
    ///     Utilize this action in scenarios where the visual feedback of selectable UI elements
    ///     must be enhanced by dynamically changing the font color based on state changes.
    /// </remarks>
    public class ColorFontSelectableAction : ColorFontAction<SelectableState, ColorFontDataSelectableAction>
    {
    }
}
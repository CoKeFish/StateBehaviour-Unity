using System;
using Marmary.StateBehavior.Runtime.Actions;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    /// Represents an action applied to a 2D light component based on selectable states.
    /// This action adjusts the light's behavior dynamically depending on the current selectable state.
    /// </summary>
    /// <remarks>
    /// This class derives from Light2DAction, utilizing the SelectableState enum and associated action data.
    /// It enables modifications to a Light2D component synchronized with the states like Normal, Highlighted,
    /// PressedInside, and PressedOutside.
    /// </remarks>
    [Serializable]
    public class Light2D2DSelectableAction : Light2DAction<SelectableState, Light2DDataSelectableAction>
    {
    }
}
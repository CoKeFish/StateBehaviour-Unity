using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Represents selectable animation font data action mapped to different <see cref="SelectableState" /> values.
    /// </summary>
    /// <remarks>
    ///     This class is used to define animations or changes in font data that are specific to each selectable state
    ///     within a UI or interactive context.
    /// </remarks>
    /// <example>
    ///     This class serves as a configuration object for UI state transitions, enabling developers to assign specific
    ///     font data modifications to states like Normal, Highlighted, PressedInside, and PressedOutside.
    /// </example>
    [CreateAssetMenu(fileName = "AnimationFontDataSelectableAction",
        menuName = "StateBehavior/Actions/AnimationFontDataSelectableAction",
        order = 1)]
    public class AnimationFontDataSelectableAction : ActionDataSelectable<float>
    {
    }
}
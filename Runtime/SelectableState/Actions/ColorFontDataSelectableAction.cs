#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Scriptable object designed to manage and store color data associated with various selectable states
    ///     for text-based interactions.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorFontDataSelectableAction",
        menuName = "StateBehavior/Actions/ColorFontDataSelectableAction",
        order = 1)]
    public class ColorFontDataSelectableAction : ActionDataSelectable<Color>
    {
    }
}
#endif
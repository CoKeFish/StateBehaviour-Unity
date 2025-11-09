#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    /// <summary>
    ///     Scriptable object that stores color targets for text-based selectable actions.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorFontSelectableActionData",
        menuName = "StateBehavior/Actions/ColorFontSelectableActionData",
        order = 1)]
    public class ColorFontSelectableActionData : ActionDataSelectable<Color>
    {
    }
}
#endif
#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Scriptable object that stores color targets for sprite-based selectable actions.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorSpriteDataSelectableAction",
        menuName = "StateBehavior/Actions/ColorSpriteDataSelectableAction",
        order = 1)]
    public class ColorSpriteDataSelectableAction : ActionDataSelectable<Color>
    {
    }
}
#endif
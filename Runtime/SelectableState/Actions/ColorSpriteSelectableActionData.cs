#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    /// <summary>
    ///     Scriptable object that stores color targets for sprite-based selectable actions.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorSpriteSelectableActionData",
        menuName = "StateBehavior/Actions/ColorSpriteSelectableActionData",
        order = 1)]
    public class ColorSpriteSelectableActionData : ActionDataSelectable<Color>
    {
    }
}
#endif
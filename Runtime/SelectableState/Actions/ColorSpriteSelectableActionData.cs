#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    [CreateAssetMenu(fileName = "ColorSpriteSelectableActionData",
        menuName = "StateBehavior/Actions/ColorSpriteSelectableActionData",
        order = 1)]
    public class ColorSpriteSelectableActionData : ActionDataSelectable<Color>
    {
    }
}
#endif
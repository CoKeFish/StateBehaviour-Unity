#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState.Actions
{
    [CreateAssetMenu(fileName = "ScaleSelectableActionData",
        menuName = "StateBehavior/Actions/ScaleSelectableActionData",
        order = 1)]
    public class ScaleSelectableActionData : ActionDataSelectable<Vector3>
    {
    }
}
#endif
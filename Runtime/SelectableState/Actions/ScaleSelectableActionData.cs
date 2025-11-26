#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Scriptable object containing per-state scale targets for selectable elements.
    /// </summary>
    [CreateAssetMenu(fileName = "ScaleSelectableActionData",
        menuName = "StateBehavior/Actions/ScaleSelectableActionData",
        order = 1)]
    public class ScaleSelectableActionData : ActionDataSelectable<Vector3>
    {
    }
}
#endif
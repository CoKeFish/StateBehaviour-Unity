#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Scriptable object containing per-state scale targets for selectable elements.
    /// </summary>
    [CreateAssetMenu(fileName = "ScaleDataSelectableAction",
        menuName = "StateBehavior/Actions/ScaleDataSelectableAction",
        order = 1)]
    public class ScaleDataSelectableAction : ActionDataSelectable<Vector3>
    {
    }
}
#endif
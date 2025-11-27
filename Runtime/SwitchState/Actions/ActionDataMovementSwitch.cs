#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SwitchState.Actions
{
    /// <summary>
    ///     Action data for MovementSwitchAction that animates RectTransform movement for show and hide transitions.
    /// </summary>
    [CreateAssetMenu(fileName = "ActionDataMovementSwitch",
        menuName = "StateBehavior/SwitchState/ActionDataMovementSwitch")]
    public class ActionDataMovementSwitch : ActionDataSwitch<Vector3>
    {
    }
}
#endif
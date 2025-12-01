#if STATE_BEHAVIOR_ENABLED
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SwitchState.Actions
{
    /// <summary>
    ///     Action data for MovementSwitchAction that animates RectTransform movement for show and hide transitions.
    /// </summary>
    [CreateAssetMenu(fileName = "RectTransformMovementDataSwitchAction",
        menuName = "StateBehavior/SwitchState/RectTransformMovementDataSwitchAction")]
    public class RectTransformMovementDataSwitchAction : ActionDataSwitch<Vector3>
    {
    }
}
#endif
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SwitchState.Actions
{
    /// <summary>
    ///     Represents a scriptable object used for configuring scale transformation behavior
    ///     specific to states defined by the <see cref="SwitchState" /> enumeration.
    /// </summary>
    /// <remarks>
    ///     This class enables seamless mapping of scale values (<see cref="Vector3" />)
    ///     to corresponding states using a dictionary container. It inherits from
    ///     <see cref="ActionDataSwitch{TValue}" />, which provides generic support for state-driven animations.
    /// </remarks>
    [CreateAssetMenu(fileName = "ScaleDataSwitchAction",
        menuName = "StateBehavior/Actions/ScaleDataSwitchAction",
        order = 1)]
    public class ScaleDataSwitchAction : ActionDataSwitch<Vector3>
    {
    }
}
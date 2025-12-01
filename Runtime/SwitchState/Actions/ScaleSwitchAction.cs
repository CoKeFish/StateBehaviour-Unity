using Marmary.StateBehavior.Runtime.Actions;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SwitchState.Actions
{
    /// <summary>
    ///     Represents an action that modifies the local scale of a <see cref="RectTransform" />
    ///     in response to state changes of type <see cref="SwitchState" />.
    ///     Utilizes scale tweening to transition between states defined in <see cref="SwitchState" />,
    ///     with configuration data provided by <see cref="ScaleDataSwitchAction" />.
    /// </summary>
    public class ScaleSwitchAction : ScaleAction<SwitchState, ScaleDataSwitchAction>
    {
    }
}
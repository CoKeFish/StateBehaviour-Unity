#if STATE_BEHAVIOR_ENABLED
using System;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.SwitchState
{
    /// <summary>
    ///     Base action used by switchable UI elements to tween component values.
    /// </summary>
    /// <typeparam name="TComponent">Component that receives tweened values.</typeparam>
    /// <typeparam name="TValue">Type of value driven by the tween.</typeparam>
    [Serializable]
    public abstract class SwitchAction<TComponent, TValue> : Action<SwitchState, TValue>
        where TComponent : Component
    {
        #region Serialized Fields

        /// <summary>
        ///     Target component affected by the tween.
        /// </summary>
        [SerializeField] protected TComponent target;

        #endregion
    }
}
#endif
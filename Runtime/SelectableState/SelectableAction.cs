#if STATE_BEHAVIOR_ENABLED
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState
{
    /// <summary>
    ///     Base selectable action that manages tween lifecycle for a specific component type.
    /// </summary>
    [Serializable]
    public abstract class SelectableAction<TComponent, TValue> : Core.Action<SelectableState, TValue>
        where TComponent : Component
    {
        #region Serialized Fields

        /// <summary>
        ///     Component instance that receives the tweened values.
        /// </summary>
        [SerializeField] [PropertyOrder(-10)] protected TComponent target;

        #endregion
        
    }
}
#endif
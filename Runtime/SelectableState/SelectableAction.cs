#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
using Marmary.StateBehavior.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState
{
    /// <summary>
    ///     Base selectable action that manages tween lifecycle for a specific component type.
    /// </summary>
    [Serializable]
    public abstract class SelectableAction<TComponent, TValue> : Core.Action<SelectableState, TValue>,
        IStateContract<SelectableState> where TComponent : Component
    {
        #region Serialized Fields

        /// <summary>
        ///     Component instance that receives the tweened values.
        /// </summary>
        [SerializeField] [PropertyOrder(-10)] protected TComponent target;

        #endregion

        #region Fields

        /// <summary>
        ///     Captured value before applying action data.
        /// </summary>
        protected TValue OriginalValue;

        /// <summary>
        ///     Tween instance reused across state changes.
        /// </summary>
        protected Tweener Tweener;

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the tweener that will be reused for state transitions.
        /// </summary>
        /// <param name="gameObject">Game object hosting the component.</param>
        /// <returns>Newly created tweener.</returns>
        protected abstract Tweener CreateTweener(GameObject gameObject);

        /// <summary>
        ///     Captures the starting value from the component prior to running animations.
        /// </summary>
        /// <param name="gameObject">Game object hosting the component.</param>
        protected abstract void InitializeStartValue(GameObject gameObject);

        #endregion

        #region IStateContract<SelectableState> Members

        /// <inheritdoc />
        public void Setup(GameObject gameObject)
        {
            InitializeStartValue(gameObject);
            Tweener = CreateTweener(gameObject);
        }

        /// <inheritdoc />
        public void Set(SelectableState state)
        {
            BehaviorActionFactory.Set(data.StateActionDataContainers[state].behaviorActionType, Tweener);
            data.StateActionDataContainers[state].BehaviorActionData.ApplyData(Tweener, OriginalValue).Restart();
        }

        #endregion
    }
}
#endif
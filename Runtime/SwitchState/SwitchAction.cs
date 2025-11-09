#if STATE_BEHAVIOR_ENABLED
using System;
using DG.Tweening;
using Marmary.StateBehavior.Core;
using UnityEngine;

namespace Marmary.StateBehavior.SwitchState
{
    /// <summary>
    ///     Base action used by switchable UI elements to tween component values.
    /// </summary>
    /// <typeparam name="TComponent">Component that receives tweened values.</typeparam>
    /// <typeparam name="TValue">Type of value driven by the tween.</typeparam>
    [Serializable]
    public abstract class SwitchAction<TComponent, TValue> : Core.Action<SwitchState, TValue>,
        IStateContract<SwitchState> where TComponent : Component
    {
        #region Serialized Fields

        /// <summary>
        ///     Target component affected by the tween.
        /// </summary>
        [SerializeField] protected TComponent target;

        #endregion

        #region Fields

        /// <summary>
        ///     Cached value captured before the animation starts.
        /// </summary>
        protected TValue OriginalValue;

        /// <summary>
        ///     Tween instance reused between state changes.
        /// </summary>
        protected Tweener Tweener;

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the tween handled by the action.
        /// </summary>
        /// <param name="gameObject">Owner game object.</param>
        /// <returns>Configured tween instance.</returns>
        protected abstract Tweener CreateTweener(GameObject gameObject);

        /// <summary>
        ///     Captures the baseline value before applying animations.
        /// </summary>
        /// <param name="gameObject">Owner game object.</param>
        protected abstract void InitializeStartValue(GameObject gameObject);

        #endregion

        #region IStateContract Members

        /// <inheritdoc />
        public void Setup(GameObject gameObject)
        {
            InitializeStartValue(gameObject);
            Tweener = CreateTweener(gameObject);
        }

        /// <inheritdoc />
        public void Set(SwitchState state)
        {
            BehaviorActionFactory.Set(data.StateActionDataContainers[state].behaviorActionType, Tweener);
            data.StateActionDataContainers[state].BehaviorActionData.ApplyData(Tweener, OriginalValue).Restart();
        }

        #endregion
    }
}
#endif


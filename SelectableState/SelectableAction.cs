using System;
using DG.Tweening;
using Marmary.StateBehavior.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.SelectableState
{
    [Serializable]
    public abstract class SelectableAction<TComponent, TValue> : Core.Action<SelectableState, TValue>,
        IStateContract<SelectableState> where TComponent : Component
    {
        protected TValue originalValue;


        [SerializeField][PropertyOrder(-10)]
        protected TComponent target;

        protected Tweener Tweener;

        public void Setup(GameObject gameObject)
        {
            InitializeStartValue(gameObject);
            Tweener = CreateTweener(gameObject);
        }

        public void Set(SelectableState state)
        {
            BehaviorActionFactory.Set(Data.StateActionDataContainers[state].BehaviorActionType, Tweener);
            Data.StateActionDataContainers[state].BehaviorActionData.ApplyData(Tweener, originalValue).Restart();
        }

        protected abstract Tweener CreateTweener(GameObject gameObject);
        protected abstract void InitializeStartValue(GameObject gameObject);
    }
}
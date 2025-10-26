using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scrips.StateBehavior
{
    [Serializable]
    public class ActionDataStateWrapper<TValue>
    {
        [ValueDropdown(nameof(GetActionTypes))] [OnValueChanged(nameof(UpdateActionBase))] [SerializeField]
        public BehaviorActionTypes BehaviorActionType;

        [InlineProperty, HideLabel] [ShowInInspector]
        public ActionDataState<TValue> BehaviorActionData;

        private bool IsDefaultState = false;
        
        public ActionDataStateWrapper(bool isDefaultState = false)
        {
            BehaviorActionType = BehaviorActionTypes.None;
            IsDefaultState = isDefaultState;
        }

        // Dropdown source
        private static BehaviorActionTypes[] GetActionTypes()
        {
            return (BehaviorActionTypes[])Enum.GetValues(typeof(BehaviorActionTypes));
        }

        // Called automatically when Type changes
        private void UpdateActionBase()
        {
            BehaviorActionData = ActionDataStateFactory.Create<TValue>(BehaviorActionType, IsDefaultState);
        }
    }
}
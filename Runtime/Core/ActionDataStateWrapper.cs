#if STATE_BEHAVIOR_ENABLED
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Core
{
    [Serializable]
    public class ActionDataStateWrapper<TValue>
    {
        #region Serialized Fields

        [ValueDropdown(nameof(GetActionTypes))] [OnValueChanged(nameof(UpdateActionBase))] [SerializeField]
        public BehaviorActionTypes BehaviorActionType;

        #endregion

        #region Fields

        private bool IsDefaultState;

        [InlineProperty] [HideLabel] [ShowInInspector]
        public ActionDataState<TValue> BehaviorActionData;

        #endregion

        #region Constructors and Injected

        public ActionDataStateWrapper(bool isDefaultState = false)
        {
            BehaviorActionType = BehaviorActionTypes.None;
            IsDefaultState = isDefaultState;
        }

        #endregion

        #region Methods

        // Called automatically when Type changes
        private void UpdateActionBase()
        {
            BehaviorActionData = ActionDataStateFactory.Create<TValue>(BehaviorActionType, IsDefaultState);
        }

        // Dropdown source
        private static BehaviorActionTypes[] GetActionTypes()
        {
            return (BehaviorActionTypes[])Enum.GetValues(typeof(BehaviorActionTypes));
        }

        #endregion
    }
}
#endif
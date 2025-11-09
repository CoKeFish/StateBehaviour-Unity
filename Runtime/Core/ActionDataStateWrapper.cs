#if STATE_BEHAVIOR_ENABLED
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Wraps the configuration for a state, including the selected behavior type and its data instance.
    /// </summary>
    [Serializable]
    public class ActionDataStateWrapper<TValue>
    {
        #region Serialized Fields

        /// <summary>
        ///     Behavior type assigned to the state.
        /// </summary>
        [FormerlySerializedAs("BehaviorActionType")] [ValueDropdown(nameof(GetActionTypes))] [OnValueChanged(nameof(UpdateActionBase))] [SerializeField]
        public BehaviorActionTypes behaviorActionType;

        #endregion

        #region Fields

        /// <summary>
        ///     Tracks whether the wrapper corresponds to the default state.
        /// </summary>
        private bool _isDefaultState;

        /// <summary>
        ///     Instance containing the configuration for the selected behavior type.
        /// </summary>
        [InlineProperty] [HideLabel] [ShowInInspector]
        public ActionDataState<TValue> BehaviorActionData;

        #endregion

        #region Constructors and Injected

        /// <summary>
        ///     Creates a wrapper for a state configuration.
        /// </summary>
        /// <param name="isDefaultState">Indicates whether the state is the enum default value.</param>
        public ActionDataStateWrapper(bool isDefaultState = false)
        {
            behaviorActionType = BehaviorActionTypes.None;
            _isDefaultState = isDefaultState;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Instantiates the internal action data when the behavior type changes.
        /// </summary>
        private void UpdateActionBase()
        {
            BehaviorActionData = ActionDataStateFactory.Create<TValue>(behaviorActionType, _isDefaultState);
        }

        /// <summary>
        ///     Retrieves the available behavior types.
        /// </summary>
        /// <returns>Array containing all values of <see cref="BehaviorActionTypes"/>.</returns>
        private static BehaviorActionTypes[] GetActionTypes()
        {
            return (BehaviorActionTypes[])Enum.GetValues(typeof(BehaviorActionTypes));
        }

        #endregion
    }
}
#endif
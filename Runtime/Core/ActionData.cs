#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Base container for mapping state values to <see cref="ActionDataState{TValue}"/> instances.
    /// </summary>
    [Serializable]
    public class ActionData<TState, TValue> : ActionDataBase
        where TState : Enum
    {
        #region Serialized Fields

        /// <summary>
        ///     Gets or sets the mapping of state entries to their action configuration.
        /// </summary>
        [DictionaryDrawerSettings(KeyLabel = "State", ValueLabel = "Data",
            DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<TState, ActionDataStateWrapper<TValue>> StateActionDataContainers = new();

        #endregion

        #region Unity Event Functions

        /// <summary>
        ///     Ensures the state containers exist when the asset is loaded.
        /// </summary>
        private void OnEnable() => EnsureStateActionDataContainers();

        /// <summary>
        ///     Ensures the state containers remain in sync when the asset is validated in the editor.
        /// </summary>
        private void OnValidate() => EnsureStateActionDataContainers();

        /// <summary>
        ///     Synchronises the backing dictionary with the values defined in the <typeparamref name="TState"/> enum.
        /// </summary>
        private void EnsureStateActionDataContainers()
        {
            var states = Enum.GetValues(typeof(TState)).Cast<TState>().ToArray();

            foreach (var state in states)
            {
                if (Convert.ToInt32(state) == 0)
                {
                    if (!StateActionDataContainers.ContainsKey(state))
                        StateActionDataContainers[state] = new ActionDataStateWrapper<TValue>(true);
                    continue;
                }

                if (!StateActionDataContainers.ContainsKey(state))
                    StateActionDataContainers[state] = new ActionDataStateWrapper<TValue>();
            }

            var keysToRemove = StateActionDataContainers.Keys
                .Where(k => !states.Contains(k))
                .ToList();

            foreach (var key in keysToRemove)
                StateActionDataContainers.Remove(key);
        }

        #endregion
    }
}
#endif